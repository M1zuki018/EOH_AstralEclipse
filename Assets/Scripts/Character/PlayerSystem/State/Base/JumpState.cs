using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// ジャンプ状態
    /// </summary>
    public class JumpState : PlayerBaseState<BaseStateEnum>
    {
        public JumpState(IPlayerStateMachine stateMachine) : base(stateMachine) { }
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            Debug.Log("JumpState: Enter");
            
            //TODO: アニメーション再生

            ActionHandler.Jump();
            BlackBoard.ApplyGravity = true;
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Jump)
            {
                //TODO: ジャンプが終わった時の入力量に応じてIdle/Moveに遷移する
                if (BlackBoard.IsGrounded && BlackBoard.Velocity.y < 0)
                {
                    // 移動入力がなければ Idle へ
                    if (BlackBoard.MoveDirection.sqrMagnitude < 0.01f)
                    {
                        StateMachine.ChangeState(BaseStateEnum.Idle);
                        return;
                    }
                    // 移動入力があれば Move へ
                    else
                    {
                        StateMachine.ChangeState(BaseStateEnum.Move);
                        return;
                    }
                }
                
                // スキル入力があれば Skill へ
                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Skill))
                {
                    StateMachine.ChangeState(BaseStateEnum.Skill);
                    return;
                }
                
                // 攻撃入力があれば Attack へ
                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Attack))
                {
                    StateMachine.ChangeState(BaseStateEnum.Attack);
                    return;
                }

                //　ステップ入力があり、ステップ回数がゼロ以上あったら Step へ
                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Step))
                {
                    if (BlackBoard.CurrentSteps > 0)
                    {
                        StateMachine.ChangeState(BaseStateEnum.Step);
                    }
                    else
                    {
                        Debug.Log("ステップカウントが足りません！");
                    }
                    return;
                }
                
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            // ジャンプ終了処理
            BlackBoard.IsJumping = false;
            BlackBoard.Velocity = new Vector3(0, -0.1f, 0); //確実に地面につくように少し下向きの力を加える
            BlackBoard.ApplyGravity = false;
            
            await UniTask.Yield();
        }
    }
}
