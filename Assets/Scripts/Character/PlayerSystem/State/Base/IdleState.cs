using Cysharp.Threading.Tasks;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 待機状態
    /// </summary>
    public class IdleState : PlayerBaseState<BaseStateEnum>
    {
        public IdleState(IPlayerStateMachine stateMachine) : base(stateMachine) { }
        
        private bool _isJumping = false;
        private bool _isAttacking = false;
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            //TODO: 待機アニメーション再生

            InputProcessor.OnJump += () => _isJumping = true;
            InputProcessor.OnAttack += () => _isAttacking = true;
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Idle)
            {
                // 移動入力があれば Walk へ
                if (BlackBoard.MoveDirection.magnitude > 0)
                {
                    StateMachine.ChangeState(BaseStateEnum.Walk);
                    return;
                }

                // ジャンプ入力があれば Jump へ
                if (_isJumping)
                {
                    StateMachine.ChangeState(BaseStateEnum.Jump);
                    return;
                }

                // 攻撃入力があれば Attack へ
                if (_isAttacking)
                {
                    StateMachine.ChangeState(BaseStateEnum.Attack);
                    return;
                }

                // フレームを待つ
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            InputProcessor.OnJump -= () => _isJumping = false;
            InputProcessor.OnAttack -= () => _isAttacking = false;
            
            await UniTask.CompletedTask;
        }
    }

}