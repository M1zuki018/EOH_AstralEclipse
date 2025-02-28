using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// Will値がなくなり行動不能な状態
    /// </summary>
    public class GuardBreakState : PlayerBaseState<BaseStateEnum>
    {
        public GuardBreakState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            BlackBoard.AnimController.Movement.PlayGuardBreakAnimation();
            
            await UniTask.Delay(TimeSpan.FromSeconds(3f));

            BlackBoard.IsGuardBreak = false; // ガードブレイク解除
            BlackBoard.AnimController.Movement.StopGuardBreakAnimation();
            
            StateMachine.ChangeState(BaseStateEnum.Idle);
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.GuardBreak)
            {
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