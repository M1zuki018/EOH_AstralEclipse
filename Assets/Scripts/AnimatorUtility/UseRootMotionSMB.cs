using UnityEngine;

/// <summary>
/// 攻撃モーションからの遷移後に使用
/// Locoモーションが再生されたときにルートモーションをtrueにする
/// </summary>
public class UseRootMotionSMB : StateMachineBehaviour
{
    private PlayerController _playerController;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_playerController == null) _playerController = animator.GetComponent<PlayerController>();
        
        _playerController.PlayerState.IsAttacking = false;
        animator.applyRootMotion = true;
    }
}
