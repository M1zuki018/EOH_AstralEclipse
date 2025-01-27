using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ステートから抜けたらTriggerをOffにする
/// </summary>
public class ResetTriggerOnStateExit : StateMachineBehaviour
{
    [SerializeField] private string _triggerName;
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(_triggerName);
    }
}
