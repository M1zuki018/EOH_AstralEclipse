using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ボスのパターン2-4撃目の攻撃
/// </summary>
public class BossAttack_Fourth : AttackAdjustBase
{
    [SerializeField] private HitDetectionInfo _hitDetectionInfo; 
    [SerializeField] private ShadowAttack _shadowAttack;
    private Transform _player;
    
    public override async void StartAttack()
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
            _animator.applyRootMotion = true;
        }
        
        await UniTask.Delay(300);
        
        _hitDetector.DetectHit(_hitDetectionInfo); //当たり判定を発生させる
        
        await UniTask.Delay(700);
        
        _shadowAttack.WarpToPosition();
    }

    public override void CorrectMovement(Vector3 forwardDirection) { }
    public override void CancelAttack()
    {
        
    }
}
