using UnityEngine;

/// <summary>
/// 攻撃モーションからの遷移後に使用
/// Locoモーションが再生されたときにルートモーションをtrueにする
/// </summary>
public class UseRootMotionSMB : StateMachineBehaviour
{
    private PlayerBrain _playerBrain;
    private CombatAnimationHandler _animationHandler;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // nullの時だけ取得する
        if(_playerBrain == null) _playerBrain = animator.GetComponent<PlayerBrain>();
        if (_animationHandler == null)　_animationHandler = animator.GetComponent<CombatAnimationHandler>();
        
        // Attack状態が解除されていなかったときの保険
        // （ゲーム開始時にステートがもどらないバグ対策。攻撃が落下によってキャンセルされると起こっている可能性）
        _animationHandler.AttackFinish();
        
        animator.applyRootMotion = true;
    }
}
