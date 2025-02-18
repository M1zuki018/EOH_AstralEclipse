using Cysharp.Threading.Tasks;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// ダッシュ回避状態
    /// </summary>
    public class StepState : BaseState<BaseStateEnum>
    {
        public StepState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override UniTask Enter()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override UniTask Execute()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}
