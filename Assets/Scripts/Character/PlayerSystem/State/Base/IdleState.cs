using Cysharp.Threading.Tasks;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 待機状態
    /// </summary>
    public class IdleState : BaseState<BaseStateEnum>
    {
        public IdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            //TODO: 待機アニメーション再生
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Idle)
            {
                /*
                // 移動入力があれば Walk へ
                if (stateMachine.Input.MoveInput.magnitude > 0)
                {
                    StateMachine.ChangeState(BaseStateEnum.Walk);
                    return;
                }

                // ジャンプ入力があれば Jump へ
                if (stateMachine.Input.JumpInput)
                {
                    StateMachine.ChangeState(BaseStateEnum.Jump);
                    return;
                }

                // 攻撃入力があれば Attack へ
                if (stateMachine.Input.AttackInput)
                {
                    StateMachine.ChangeState(BaseStateEnum.Attack);
                    return;
                }
                */

                // フレームを待つ
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            await UniTask.CompletedTask;
        }
    }

}