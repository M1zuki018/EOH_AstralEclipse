using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 通常攻撃3段目の補正
/// </summary>
public class NormalAttack_Kick : AttackAdjustBase
{
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;
    
    private CancellationTokenSource _cts;
    private bool _isAttacking;

    public override async void StartAttack()
    {
        _target = _adjustDirection.Target;
        _isAttacking = true;
        _cts = new CancellationTokenSource();
        
        if (_target != null)
        {
            _adjustDirection.AdjustDirectionToTargetEarly();
            _animator.applyRootMotion = true;
        }
        else
        {
            _animator.applyRootMotion = true;
        }

        try
        {
            await UniTask.Delay(50);
     
            AudioManager.Instance?.PlaySE(5);
        
            await UniTask.Delay(80);
        
            _hitDetector.DetectHit(_hitDetectionInfo); //当たり判定を発生させる
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

    public override void CorrectMovement(Vector3 forwardDirection){ }
    /// <summary>
    /// 攻撃処理を中断したい時に呼ぶMethod
    /// </summary>
    public override void CancelAttack()
    {
        if (_isAttacking && _cts != null)
        {
            _cts.Cancel();
            Debug.Log("攻撃がキャンセルされました");
        }
    }
}
