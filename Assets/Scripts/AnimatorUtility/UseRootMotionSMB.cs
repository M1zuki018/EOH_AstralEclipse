using UnityEngine;

/// <summary>
/// 攻撃モーションからの遷移後に使用
/// Locoモーションが再生されたときにルートモーションをtrueにする
/// </summary>
public class UseRootMotionSMB : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = true;
    }
}
