using Cysharp.Threading.Tasks;

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
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            await UniTask.Yield();
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
