using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// ダッシュ回避状態
    /// </summary>
    public class StepState : PlayerBaseState<BaseStateEnum>
    {
        public StepState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            Debug.Log("Step State: Enter");
            //TODO: アニメーション再生
            
            ActionHandler.Step(); // ステップ
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f)); // 0.5秒待つ
            
            // 移動入力がなくなれば Idle へ。入力があれば Move 遷移する
            if (BlackBoard.MoveDirection.sqrMagnitude < 0.01f)
            {
                StateMachine.ChangeState(BaseStateEnum.Idle);
                return;
            }
            else
            {
                StateMachine.ChangeState(BaseStateEnum.Move);
                return;
            }
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
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
