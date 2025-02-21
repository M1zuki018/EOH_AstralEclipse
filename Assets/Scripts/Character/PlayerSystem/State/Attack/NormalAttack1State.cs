using Cysharp.Threading.Tasks;

namespace PlayerSystem.State.Attack 
{
    /// <summary>
    /// NormalAttack1状態 
    /// </summary>
    public class NormalAttack1State : PlayerBaseState<AttackStateEnum> 
    {
        public NormalAttack1State(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == AttackStateEnum.NormalAttack1)
            {
                await UniTask.Yield();
            }
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