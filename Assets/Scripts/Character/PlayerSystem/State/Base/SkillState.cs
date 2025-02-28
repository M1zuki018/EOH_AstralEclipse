using Cysharp.Threading.Tasks;
using PlayerSystem.State.Base;
using UnityEngine;

/// <summary>
/// スキル発動状態
/// </summary>
public class SkillState : PlayerBaseState<BaseStateEnum>
{
    public SkillState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

    /// <summary>
    /// ステートに入るときの処理
    /// </summary>
    public override async UniTask Enter()
    {
        BlackBoard.AttackFinishedTrigger = false; //TODO: Skillステートに入った時になぜかAttackFinishedTriggerがtrueになっているバグ
        
        // スキルが使えるか判定する
        if (ActionHandler.CanUseSkill)
        {
            await ActionHandler.Skill(); // 発動
        }
        
        await UniTask.Yield();
    }

    
    /// <summary>
    /// 毎フレーム呼ばれる処理（状態遷移など）
    /// </summary>
    public override async UniTask Execute()
    {
        if (BlackBoard.AttackFinishedTrigger)
        {
            BlackBoard.AttackFinishedTrigger = false;
            StateMachine.ChangeState(BaseStateEnum.Idle);
        }
        
        await UniTask.Yield();
    }

    /// <summary>
    /// ステートから出るときの処理
    /// </summary>
    public override async UniTask Exit()
    {
        await UniTask.Yield();
    }
}
