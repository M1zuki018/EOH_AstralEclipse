using UnityEngine;

/// <summary>
/// ステートから抜けたタイミングでTriggerをOffにする
/// 一つのアニメーションのステートを抜けたタイミングで使う
/// </summary>
public class ResetTriggerOnStateExitSMB : StateMachineBehaviour
{
    [SerializeField] private string _triggerName;
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(_triggerName);
    }
}
