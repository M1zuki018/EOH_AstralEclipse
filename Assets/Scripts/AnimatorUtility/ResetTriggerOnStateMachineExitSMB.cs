using UnityEngine;

/// <summary>
/// ステートマシンから抜けたらTriggerをOffにする
/// サブステートマシン全体を抜けた時にリセットする場合に使う
/// </summary>
public class ResetTriggerOnStateMachineExitSMB : StateMachineBehaviour
{
    [SerializeField] string _triggerName;

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        animator.ResetTrigger(_triggerName);
    }
}