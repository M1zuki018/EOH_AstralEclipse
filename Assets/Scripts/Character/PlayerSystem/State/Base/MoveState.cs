using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 移動状態
    /// </summary>
    public class MoveState : PlayerBaseState<BaseStateEnum>, IFixedUpdateState
    {
        public MoveState(IPlayerStateMachine stateMachine) : base(stateMachine) { }
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            BlackBoard.ApplyGravity = true;
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            await UniTask.Yield();
            
            while (StateMachine.CurrentState.Value == BaseStateEnum.Move)
            {
                // ジャンプ入力があり、地面についていた場合 Jump へ
                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Jump) && BlackBoard.IsGrounded)
                {
                    StateMachine.ChangeState(BaseStateEnum.Jump);
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
                
                // スキル入力があれば Skill へ
                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Skill))
                {
                    StateMachine.ChangeState(BaseStateEnum.Skill);
                    return;
                }

                // 攻撃入力があれば NormalAttack へ
                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Attack))
                {
                    StateMachine.ChangeState(BaseStateEnum.NormalAttack);
                    return;
                }

                // 防御入力があれば Guard へ
                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Guard))
                {
                    StateMachine.ChangeState(BaseStateEnum.Guard);
                    return;
                }
                
                // 移動入力がなくなれば Idle へ
                if (BlackBoard.MoveDirection.sqrMagnitude < 0.01f)
                {
                    StateMachine.ChangeState(BaseStateEnum.Idle);
                    return;
                }

                await UniTask.Yield();
            }
        }
        
        public override async UniTask FixedExecute()
        {
            await UniTask.Yield();
            
            /* //TODO:FixedUpdateで更新しているはずなのに、ここで呼ぶFixedUpdateと外のものの振る舞いが違う
            while (StateMachine.CurrentState.Value == BaseStateEnum.Move)
            {
                //ActionHandler.Move();
                
                // 移動入力がなくなれば Idle へ
                if (BlackBoard.MoveDirection.sqrMagnitude < 0.01f)
                {
                    StateMachine.ChangeState(BaseStateEnum.Idle);
                    return;
                }

                // ジャンプ入力があれば Jump へ
                if (_isJumping)
                {
                    StateMachine.ChangeState(BaseStateEnum.Jump);
                    return;
                }

                // 攻撃入力があれば NormalAttack へ
                if (_isAttacking)
                {
                    StateMachine.ChangeState(BaseStateEnum.NormalAttack);
                    return;
                }

                // 防御入力があれば Guard へ
                if (_isGuard)
                {
                    StateMachine.ChangeState(BaseStateEnum.Guard);
                    return;
                }

                await UniTask.Yield();
            }
            */
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
