using System;
using Cysharp.Threading.Tasks;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 通常攻撃状態
    /// </summary>
    public class NormalAttackState : PlayerBaseState<BaseStateEnum>
    {
        public NormalAttackState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            BlackBoard.ApplyGravity = true;
            ActionHandler.Attack();

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
                return;
            }

            if (!BlackBoard.IsGrounded)
            {
                StateMachine.ChangeState(BaseStateEnum.AirAttack);
                return;
            }
            
            await UniTask.Yield();
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            BlackBoard.ApplyGravity = false;
            
            await UniTask.Yield();
        }
    }

}