using UnityEngine;

/// <summary>
/// 敵のジャンプ攻撃の補正クラス
/// </summary>
public class EnemyNormalAttack_JumpAttack : AttackAdjustBase
{
    [Header("初期設定")]
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;
    
    public override void StartAttack()
    {
    }

    public override void CorrectMovement(Vector3 forwardDirection) {　}
}
