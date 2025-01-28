using UnityEngine;

/// <summary>
/// 攻撃モーションからの遷移後に使用
/// Locoモーションが再生されたときにルートモーションをtrueにする
/// </summary>
public class UseRootMotionSMB : StateMachineBehaviour
{
    private PlayerMovement _playerMovement;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_playerMovement == null) _playerMovement = animator.GetComponent<PlayerMovement>();
        
        _playerMovement.PlayerState.IsAttacking = false;
        animator.applyRootMotion = true;
    }
}
