using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ボスのパターン2-4撃目の攻撃
/// </summary>
public class BossAttack_Fourth : AttackAdjustBase
{
    [SerializeField] private HitDetectionInfo _hitDetectionInfo; 
    private Transform _player;
    private BossMover _bossMover;
    
    public override async void StartAttack()
    {
        if(_player == null) _player = GameObject.FindGameObjectWithTag("Player").transform; 
        if(_bossMover == null) _bossMover = GetComponent<BossMover>();
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
        
        //_bossMover.TransitionPattern2(); //次の攻撃に遷移する
    }

    public override void CorrectMovement(Vector3 forwardDirection) { }
    public override void CancelAttack()
    {
        
    }
}
