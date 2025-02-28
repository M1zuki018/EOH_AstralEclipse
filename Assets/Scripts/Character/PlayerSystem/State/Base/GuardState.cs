using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// ガード状態
    /// </summary>
    public class GuardState : PlayerBaseState<BaseStateEnum>
    {
        public GuardState(IPlayerStateMachine stateMachine) : base(stateMachine) { }
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            Debug.Log("Guard State: Enter");

            BlackBoard.ApplyGravity = true;
            ActionHandler.GuardStart();
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Guard)
            {
                ActionHandler.Guard();

                if (BlackBoard.IsGuardBreak)
                {
                    StateMachine.ChangeState(BaseStateEnum.GuardBreak);
                    return;
                }

                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Guard)) // ガードをやめる
                {
                    // 移動入力がなければ Idle へ
                    if (BlackBoard.MoveDirection.sqrMagnitude < 0.01f)
                    {
                        StateMachine.ChangeState(BaseStateEnum.Idle);
                        return;
                    }
                    
                    // 移動入力があれば Move へ
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
            ActionHandler.GuardEnd();
            
            await UniTask.Yield();
        }
    }
}
