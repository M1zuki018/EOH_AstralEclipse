using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 通常攻撃3段目の補正
/// </summary>
public class NormalAttack_Kick : AttackAdjustBase
{
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;   

    public override async void StartAttack()
    {
        _target = _adjustDirection.Target;
        
        if (_target != null)
        {
            _adjustDirection.AdjustDirectionToTargetEarly();
            _animator.applyRootMotion = true;
        }
        else
        {
            _animator.applyRootMotion = true;
        }
        
        await UniTask.Delay(50);
     
        AudioManager.Instance?.PlaySE(5);
        
        await UniTask.Delay(80);
        
        _hitDetector.DetectHit(_hitDetectionInfo); //当たり判定を発生させる
    }

    public override void CorrectMovement(Vector3 forwardDirection){ }
    public override void CancelAttack()
    {
        
    }
}
