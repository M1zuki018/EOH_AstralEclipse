using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 敵の殴り攻撃の補正クラス
/// </summary>
public class EnemyNormalAttack_Swiping : AttackAdjustBase
{
    [Header("初期設定")]
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;
    private Transform _player;
    
    public override async void StartAttack()
    {
        if(_player == null) _player = GameObject.FindGameObjectWithTag("Player").transform; 
        Debug.Log("開始");
        
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
        
        await UniTask.DelayFrame(150);
        
        Debug.Log("判定");
        _hitDetector.DetectHit(_hitDetectionInfo); //当たり判定を発生させる
    }

    public override void CorrectMovement(Vector3 forwardDirection) { }
}
