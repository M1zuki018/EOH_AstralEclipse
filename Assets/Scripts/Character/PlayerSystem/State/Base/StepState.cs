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
            
            // BlackBoard.ApplyGravity = true; // 多少浮く場合があるので重力を加えておく
            ActionHandler.Step(); // ステップ
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            BlackBoard.ApplyGravity = true; // ステップが終わったら重力をかけ始める
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {

            if (BlackBoard.IsGrounded)
            {
                // ジャンプ入力があったら Jump へ
                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Jump))
                {
                    StateMachine.ChangeState(BaseStateEnum.Jump);
                    return;
                }
            
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
            
            await UniTask.Yield();
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            // ステップ終了処理
            BlackBoard.ApplyGravity = false;
            BlackBoard.Velocity = new Vector3(0, -0.1f, 0); //確実に地面につくように少し下向きの力を加える
            
            await UniTask.Yield();
        }
    }
}
