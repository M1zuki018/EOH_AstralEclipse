using Cysharp.Threading.Tasks;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 演出状態
    /// </summary>
    public class PerformanceState : IState
    {
        
        private readonly PlayerStateMachine _smm;

        /// <summary>
        /// 初期化
        /// </summary>
        public PerformanceState(PlayerStateMachine smm)
        {
            _smm = smm;
        }

        public async UniTask Enter()
        {
            // Idle状態に入るときの処理（例えばアニメーションの開始など）
            _smm.SetIdleAnimation();
            await UniTask.CompletedTask;
        }

        public async UniTask Execute()
        {
            // 毎フレーム呼ばれる処理（状態遷移など）
            if (_smm.IsRunning)
            {
                //_smm.TransitionToState(new RunningState(_smm));  // RunningStateへの遷移
            }
            else if (_smm.IsAttacking)
            {
                //_smm.TransitionToState(new AttackState(_smm));  // AttackStateへの遷移
            }
            await UniTask.CompletedTask;
        }

        public async UniTask Exit()
        {
            await UniTask.CompletedTask;
        }
    }

}