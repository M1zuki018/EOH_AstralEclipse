using UnityEngine;

/// <summary>
/// ステートから抜けたらTriggerをOffにする
/// </summary>
public class ResetTriggerOnStateExit : StateMachineBehaviour
{
    [SerializeField] string _triggerName;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(_triggerName);
    }
}
