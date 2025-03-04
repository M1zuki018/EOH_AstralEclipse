using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 通常攻撃最終段の補正
/// </summary>
public class NormalAttack_End : AttackAdjustBase
{
    [SerializeField] private HitDetectionInfo[] _hitDetectionInfo; 
    [SerializeField] private EffectPositionInfo[] _effectPositionInfo;
    [SerializeField, Comment("これ以上近付かない距離")] private float _stopDistance = 1.8f;
    private Vector3 _lastValidPosition; //敵に近付きすぎたときの座標
    
    private CancellationTokenSource _cts;
    private bool _isAttacking;
    
    public override async void StartAttack()
    {
        _lastValidPosition = transform.position; //初期化
        _target = _adjustDirection.Target;
        _isAttacking = true;
        _cts = new CancellationTokenSource();

        //ターゲットが存在する場合
        if (_target != null)
        {
            _animator.applyRootMotion = false;
            _adjustDirection.AdjustDirectionToTargetEarly(); //体の向きをターゲットの方向へ向ける
            _animator.applyRootMotion = true;
        }
        else
        {
            _animator.applyRootMotion = true;
        }

        try
        {
            _animator.SetFloat("AttackSpeed", 1f);
            AudioManager.Instance?.PlaySEDelay(3, 100); //右上から切り降ろす
            AudioManager.Instance?.PlaySEDelay(3, 330); //切りながらジャンプ
        
            await UniTask.Delay(80);
            _effectPool.GetEffect(_effectPositionInfo[0].Position, _effectPositionInfo[0].Rotation);
            _hitDetector.DetectHit(_hitDetectionInfo[0]);
        
            await UniTask.Delay(250);
        
            _animator.SetFloat("AttackSpeed", 1.8f);
            if(_hitDetector.IsHit()) CameraManager.Instance?.TurnEffect();
        
            await UniTask.Delay(80);
        
            _effectPool.GetEffect(_effectPositionInfo[1].Position, _effectPositionInfo[1].Rotation);
            _hitDetector.DetectHit(_hitDetectionInfo[1]);
        
            await UniTask.Delay(150);
        
            _animator.SetFloat("AttackSpeed", 2f);
        
            await UniTask.Delay(220);
        
            
            AudioManager.Instance?.PlaySEDelay(4, 150); //着地
            _animator.SetFloat("AttackSpeed", 1f);
            if(_hitDetector.IsHit()) CameraManager.Instance?.ApplyHitStopWithEffects(0.006f);
            CameraManager.Instance?.EndDashEffect();
        
            await UniTask.Delay(200);
        
            _effectPool.GetEffect(_effectPositionInfo[2].Position, _effectPositionInfo[2].Rotation);
            
            await UniTask.Delay(100);
           
            _hitDetector.DetectHit(_hitDetectionInfo[2]); 
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

    public override void CorrectMovement(Vector3 forwardDirection)
    {
        //ターゲットがいなかったら以降の処理は行わない
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
