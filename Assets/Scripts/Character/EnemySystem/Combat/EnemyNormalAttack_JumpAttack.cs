using UnityEngine;

/// <summary>
/// 敵のジャンプ攻撃の補正クラス
/// </summary>
public class EnemyNormalAttack_JumpAttack : AttackAdjustBase
{
    [Header("初期設定")]
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;
    private Transform _player;
    
    public override void StartAttack()
    {
        if(_player == null) _player = GameObject.FindGameObjectWithTag("Player").transform; 
        
        _target = _player;
        
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
