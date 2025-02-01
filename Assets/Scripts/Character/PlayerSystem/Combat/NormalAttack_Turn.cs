using UnityEngine;

/// <summary>
/// 通常攻撃4段目の補正
/// </summary>
public class NormalAttack_Turn : AttackAdjustBase
{
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;   

    public override void StartAttack()
    {
        _hitDetector.DetectHit(_hitDetectionInfo.Collider, _hitDetectionInfo.Duration); //当たり判定を発生させる
        
        if (_target != null)
        {
            //TODO:ターゲットがいるときの補正処理を書く
        }
        else
        {
            _animator.applyRootMotion = true;
        }
        
    }

    public override void CorrectMovement(Vector3 forwardDirection){ }
}
