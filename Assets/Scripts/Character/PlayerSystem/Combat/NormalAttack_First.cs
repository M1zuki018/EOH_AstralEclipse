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
    [SerializeField] private float _approachSpeed = 30f; //突進速度
    [SerializeField] private float _attackDistance = 10f; //有効距離
    [SerializeField] private float _adjustDistance = 2f; //補正がかかる距離
    [SerializeField] private float _initializeAnimationSpeed = 1.3f; //初期アニメーションスピード
    [SerializeField] private float _forwardDistance = 1.5f; //ロックオンしていない時に移動する距離

    private bool _isAttacking = false; //突進中かどうか
    private float _distance; //敵との距離
    private float _totalDistanceToCover; //_distanceと_adjustDistanceの差
    
    private void Update()
    {
        if (_isAttacking && _target != null)
        {
            HandleApproach(); //突進処理
        }
    }

    /// <summary>
    /// 攻撃開始時に呼び出される処理
    /// </summary>
    public override void StartAttack()
    {
        _target = _combat.AdjustDirection.Target;
        
        if(_target!= null) _distance = Vector3.Distance(transform.position, _target.position); //敵との距離を計算
        
        if (_distance > _adjustDistance && _distance < _attackDistance) //補正がかかる距離よりも遠く、かつ有効距離内にいる場合
        {
            _totalDistanceToCover = _distance - _adjustDistance; // 距離の差を計算
            _isAttacking = true; //突進の処理を有効化
            
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

    public override void CorrectMovement(Vector3 forwardDirection) { }

    /// <summary>
    /// 突進処理
    /// </summary>
    private void HandleApproach()
    {
        _distance = Vector3.Distance(transform.position, _target.position); //距離を更新
                    
        //プレイヤーを敵に近づける
        Vector3 direction = (_target.position - transform.position).normalized;
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
        _isAttacking = false;
        _animator.SetFloat("AttackSpeed", _initializeAnimationSpeed);
        _animator.applyRootMotion = true;
        
        /*
        //移動
        float elapsedDistance = 0f; //移動した距離を記録する
        
        
         DOTween.To(
             () => elapsedDistance,
             value =>
             {
                 float delta = value - elapsedDistance; //前回との差分を計算
                 elapsedDistance = value; //現在の値を更新
                 
                 _cc.Move(transform.forward * delta); //差分だけ移動させる
             },
             _forwardDistance,
             0.5f)
             .SetEase(Ease.Linear);
        */
        
        AudioManager.Instance?.PlaySE(3);

        await UniTask.Delay(200);
        
        _hitDetector.DetectHit(_hitDetectionInfo); //当たり判定を発生させる;
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
}
