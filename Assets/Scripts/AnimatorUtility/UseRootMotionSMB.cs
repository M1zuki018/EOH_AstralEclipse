using UnityEngine;

/// <summary>
/// 攻撃モーションからの遷移後に使用
/// Locoモーションが再生されたときにルートモーションをtrueにする
/// </summary>
public class UseRootMotionSMB : StateMachineBehaviour
{
    private PlayerBrain _playerBrain;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_playerBrain == null) _playerBrain = animator.GetComponent<PlayerBrain>();
        
        _playerBrain.BB.IsAttacking = false;
        animator.applyRootMotion = true;
    }
}
