using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

/// <summary>
/// 通常攻撃1段目の補正
/// </summary>
public class NormalAttack_First : AttackAdjustBase
{
    [Header("初期設定")]
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;
    [SerializeField] private EffectPositionInfo _effectPositionInfo;
    [SerializeField] private float _approachSpeed = 30f; //突進速度
    [SerializeField] private float _attackDistance = 10f; //有効距離
    [SerializeField] private float _adjustDistance = 2f; //補正がかかる距離
    [SerializeField] private float _initializeAnimationSpeed = 1.3f; //初期アニメーションスピード
    [SerializeField, Comment("これ以上近付かない距離")] private float _stopDistance = 1.8f;
    private Vector3 _lastValidPosition; //敵に近付きすぎたときの座標

    private bool _isRush = false; //突進中かどうか
    private float _distance; //敵との距離
    private float _totalDistanceToCover; //_distanceと_adjustDistanceの差
    
    private CancellationTokenSource _cts;
    private bool _isAttacking;
    
    private void Update()
    {
        if (_isRush && _target != null)
        {
            HandleApproach(); //突進処理
        }
    }

    /// <summary>
    /// 攻撃開始時に呼び出される処理
    /// </summary>
    public override void StartAttack()
    {
        _target = _adjustDirection.Target;
        _isAttacking = true;
        _cts = new CancellationTokenSource();

        //ターゲットがいる場合のみ行う処理
        if (_target != null)
        {
            _distance = Vector3.Distance(transform.position, _target.position); //敵との距離を計算
            _adjustDirection.AdjustDirectionToTarget(30); //キャラクターを敵の方向に向ける
        }

        if (_distance > _adjustDistance && _distance < _attackDistance) //補正がかかる距離よりも遠く、かつ有効距離内にいる場合
        {
            _totalDistanceToCover = _distance - _adjustDistance; // 距離の差を計算
            _isRush = true; //突進の処理を有効化
            
            CameraManager.Instance?.DashEffect(); //ブラーなどの効果をかける
            CameraManager.Instance?.ApplyHitStop(0.007f);
            
            //突進が完了するまでアニメーションのスピードを設定する
            Observable
                .EveryUpdate()
                .Where(_ => _distance > _adjustDistance)
                .Subscribe(_ => AdjustAnimationSpeed())
                .AddTo(this);
            
            //突進が完了したら斬撃を行う
            Observable
                .EveryUpdate()
                .Where(_ => _distance <= _adjustDistance)
                .Take(1)
                .Subscribe(_ => TriggerSlash())
                .AddTo(this);
        }
        else
        {
            //振りかぶっている時間のアニメーション再生スピードを変更する
            _animator.SetFloat("AttackSpeed", 1.8f);
            
            float elapsedTime = 0;
            Observable
                .EveryUpdate()
                .TakeWhile(_ => elapsedTime < 0.05f)
                .Subscribe(_ => elapsedTime += Time.deltaTime, () => TriggerSlash())
                .AddTo(this);
        }
    }

    /// <summary>
    /// 突進処理
    /// </summary>
    private void HandleApproach()
    {
        _distance = Vector3.Distance(transform.position, _target.position); //距離を更新
                    
        //プレイヤーを敵に近づける
        Vector3 direction = (_target.position - transform.position).normalized;
        direction.y = 0;
        _cc.Move(direction * _approachSpeed * Time.deltaTime);

        //プレイヤーの向きを敵の方向へ合わせる
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }
    
    /// <summary>
    /// 斬撃モーションに移る
    /// </summary>
    private async void TriggerSlash()
    {
        try
        {
            CameraManager.Instance?.EndDashEffect(); //通常のエフェクトに戻す
            _effectPool.GetEffect(_effectPositionInfo.Position, _effectPositionInfo.Rotation);

            _isRush = false;
            _animator.SetFloat("AttackSpeed", _initializeAnimationSpeed);
            _animator.applyRootMotion = true;

            AudioManager.Instance?.PlaySE(3);

            await UniTask.Delay(200);

            _hitDetector.DetectHit(_hitDetectionInfo); //当たり判定を発生させる;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("攻撃処理がキャンセルされました");
        }
        finally
        {
            _isAttacking = false;
            _cts.Dispose();
        }
    }
    
    /// <summary>
    /// アニメーションの再生スピードを調整
    /// </summary>
    private void AdjustAnimationSpeed()
    {
        // 距離が近づくにつれてアニメーションスピードを変更
        float distanceToCover = _distance - _adjustDistance;
        
        // もし突進が完了していれば、アニメーションスピードを上げて斬撃に遷移させる
        if (distanceToCover <= 0)
        {
            _animator.SetFloat("AttackSpeed", _initializeAnimationSpeed); // 突進完了後に速やかに斬撃モーションに移行
            TriggerSlash(); // 斬撃モーションを即座に呼び出し
            return;
        }
        
        float normalizedSpeed = Mathf.Clamp01(distanceToCover / _totalDistanceToCover);
        float speedFactor = Mathf.Lerp(0f, 1f, normalizedSpeed); // 突進の進行度に合わせてスピードを調整
        
        // スピードをアニメーションに適用
        _animator.SetFloat("AttackSpeed", speedFactor); 
    }

    public override void CorrectMovement(Vector3 forwardDirection)
    {
        //ターゲットがいなかったら以降の処理は行わない
        //ボス戦開始時、ロックオン前だとここでnullReferenceExceptionが発生する
        if(_adjustDirection.Target == null) return; 
        
        // 敵との距離を測る
        float distanceToEnemy = Vector3.Distance(transform.position, _adjustDirection.Target.position);

        if (distanceToEnemy > _stopDistance)
        {
            // 敵に向かって移動（ルートモーションと補正を併用）
            Vector3 direction = (_adjustDirection.Target.position - transform.position).normalized;
            direction.y = 0; // Y方向の影響を無視
            _cc.Move(direction * Time.deltaTime);
            _lastValidPosition = transform.position; //現在の位置を更新
        }
        else
        {
            // 近づきすぎた場合、前フレームの適正な座標との差分を求めて補正
            Vector3 correction = _lastValidPosition - transform.position;
            correction.y = 0;
            _cc.Move(correction); // 差分を使って元の位置へ引き戻す
        }
    }

    /// <summary>
    /// 攻撃処理を中断したい時に呼ぶMethod
    /// </summary>
    public override void CancelAttack()
    {
        if (_isAttacking && _cts != null)
        {
            _cts.Cancel();
            CameraManager.Instance?.EndDashEffect();
            Debug.Log("攻撃がキャンセルされました");
        }
    }
}
