using UnityEngine;

/// <summary>
/// ステートから抜けたタイミングでTriggerをOffにする
/// 一つのアニメーションのステートを抜けたタイミングで使う
/// </summary>
public class ResetTriggerOnStateExitSMB : StateMachineBehaviour
{
    [SerializeField] private string _triggerName;
    private CombatAnimationHandler _animationHandler;
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_animationHandler == null)
        {
            _animationHandler = animator.GetComponent<CombatAnimationHandler>();
        }

        if (stateInfo.normalizedTime >= 1.0f)
        {
            Debug.Log("リセット");
            _animationHandler.AttackFinish(); // アニメーションが最後まで再生されたらIdle状態に戻す
        }
        
        animator.ResetTrigger(_triggerName);
    }
}
