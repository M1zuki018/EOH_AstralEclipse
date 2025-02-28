using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 待機状態
    /// </summary>
    public class IdleState : PlayerBaseState<BaseStateEnum>
    {
        public IdleState(IPlayerStateMachine stateMachine) : base(stateMachine) { }
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            //TODO: アニメーション再生

            Debug.Log("Idle entered");
            
            BlackBoard.ApplyGravity = true;
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Idle)
            {
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
                
                // ジャンプ入力があり、地面についていた場合 Jump へ
                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Jump) && BlackBoard.IsGrounded)
                {
                    StateMachine.ChangeState(BaseStateEnum.Jump);
                    return;
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

                // 防御入力があれば Guard へ
                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Guard))
                {
                    StateMachine.ChangeState(BaseStateEnum.Guard);
                    return;
                }
                
                // 移動入力があれば Move へ
                if (BlackBoard.MoveDirection.sqrMagnitude > 0.01f)
                {
                    StateMachine.ChangeState(BaseStateEnum.Move);
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
            BlackBoard.ApplyGravity = false;
            
            await UniTask.CompletedTask;
        }
    }

}