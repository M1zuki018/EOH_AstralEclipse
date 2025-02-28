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

        private int _count; // パリィ判定用のカウンター
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            BlackBoard.ApplyGravity = true;
            ActionHandler.GuardStart();
            _count = 0;
            BlackBoard.ParryReception = true; // パリィ受付開始
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Guard)
            {
                _count++;
                if (_count >= BlackBoard.Status.ParryReceptionTime)
                {
                    BlackBoard.ParryReception = false; // パリィ受付終了
                    Debug.Log("パリィ受付終了");
                }

                ActionHandler.Guard();

                if (BlackBoard.ParryReception && BlackBoard.SuccessParry)
                {
                    BlackBoard.ParryReception = false;
                    BlackBoard.SuccessParry = false;
                    StateMachine.ChangeState(BaseStateEnum.Parry);
                    return;
                }
                
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
