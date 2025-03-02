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
            ActionHandler.Step(); // ステップ

            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            if (StateMachine.CurrentState.Value == BaseStateEnum.Step)
            {
                if (!BlackBoard.IsSteping)
                {
                    // Attack ステートへ
                    if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Attack))
                    {
                        StateMachine.ChangeState(BlackBoard.IsGrounded // 地面にいるか判定
                            ? BaseStateEnum.NormalAttack // 通常攻撃
                            : BaseStateEnum.AirAttack); // 空中攻撃
                        return;
                    }
                
                    // 地面についている　かつ　ジャンプ入力があったら Jump へ
                    if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Jump) && BlackBoard.IsGrounded)
                    {
                        StateMachine.ChangeState(BaseStateEnum.Jump);
                        return;
                    }

                    // 移動入力がなくなれば Idle へ。入力があれば Move へ遷移する
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
