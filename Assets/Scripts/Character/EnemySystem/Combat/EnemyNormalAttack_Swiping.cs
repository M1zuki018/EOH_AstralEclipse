using UnityEngine;

/// <summary>
/// 敵の殴り攻撃の補正クラス
/// </summary>
public class EnemyNormalAttack_Swiping : AttackAdjustBase
{
    [Header("初期設定")]
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;
    
    public override void StartAttack()
    {
        _target = _adjustDirection.Target;
        
        if (_target != null)
        {
            _adjustDirection.AdjustDirectionToTargetEarly();
            _animator.applyRootMotion = true;
        }
        else
        {
            Debug.LogWarning($"{gameObject}：{_target}が存在しません！");
        }
        
        _hitDetector.DetectHit(_hitDetectionInfo); //当たり判定を発生させる
    }

    public override void CorrectMovement(Vector3 forwardDirection) { }
}
