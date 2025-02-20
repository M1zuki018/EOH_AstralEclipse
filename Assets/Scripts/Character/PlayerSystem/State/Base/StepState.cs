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
            //TODO: アニメーション再生
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Step)
            {
                //TODO: アニメーション終了時の移動入力量に応じてIdle/Moveに遷移する
                
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
