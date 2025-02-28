
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base 
{
    /// <summary>
    /// 空中攻撃状態 
    /// </summary>
    public class AirAttackState : PlayerBaseState<BaseStateEnum> 
    {
        public AirAttackState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            ActionHandler.Attack();
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.AirAttack)
            {
                if (BlackBoard.AttackFinishedTrigger)
                {
                    BlackBoard.AttackFinishedTrigger = false;
                    StateMachine.ChangeState(BaseStateEnum.Idle);
                }
                
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            await UniTask.Yield();
        }
    }
}