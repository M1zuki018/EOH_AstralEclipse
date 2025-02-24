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
            // nullの時だけ取得する
            _animationHandler = animator.GetComponent<CombatAnimationHandler>();
        }

        if (stateInfo.normalizedTime >= 1.0f)
        {
            // アニメーションが最後まで再生されたら＝コンボが切れたらステートマシンをIdleに戻す
            _animationHandler.AttackFinish();
        }
        
        animator.ResetTrigger(_triggerName);
    }
}
