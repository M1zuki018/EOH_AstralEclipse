using UnityEngine;

/// <summary>
/// アニメーションの途中で武器のエフェクトを終了する
/// </summary>
public class AttackEffectBehaviour : StateMachineBehaviour
{
    private IAttackEffector _effector;
    
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash) => animator.TryGetComponent(out _effector);

    /// <summary>
    /// ステートから抜けた時によばれるコールバック
    /// </summary>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //武器のエフェクトを終了する
        _effector.MeleeAttackEnd();
    }
}