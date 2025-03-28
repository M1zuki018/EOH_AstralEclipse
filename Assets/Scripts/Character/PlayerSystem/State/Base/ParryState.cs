using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// パリィ状態
    /// </summary>
    public class ParryState : PlayerBaseState<BaseStateEnum>
    {
        public ParryState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            Time.timeScale = 0.2f;
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
            
            Time.timeScale = 1;
            ActionHandler.Counter(); // カウンター処理を実行
            
            StateMachine.ChangeState(BaseStateEnum.Idle);
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Parry)
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
