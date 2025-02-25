using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 攻撃状態
    /// </summary>
    public class AttackState : PlayerBaseState<BaseStateEnum>
    {
        public AttackState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            Debug.Log("AttackState : Enter");
            
            if (BlackBoard.IsGrounded)
            {
                // 地面にいる場合は通常攻撃①
                PlayerStateMachine.Animator.SetTrigger("Attack");
                return;
            }
            
            // 空中にいる場合は空中①
            PlayerStateMachine.Animator.SetTrigger("AttackAir");

            await UniTask.Delay(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            if (BlackBoard.AttackFinishedTrigger)
            {
                StateMachine.ChangeState(BaseStateEnum.Idle);
                BlackBoard.AttackFinishedTrigger = false;
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

}