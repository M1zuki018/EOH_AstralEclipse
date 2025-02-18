using System;
using Cysharp.Threading.Tasks;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 攻撃状態
    /// </summary>
    public class AttackState : BaseState<BaseStateEnum>
    {
        public AttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override UniTask Enter()
        {
            return UniTask.Delay(TimeSpan.FromSeconds(0.5f)) // 0.5秒後にIdleへ
                .ContinueWith(() => StateMachine.ChangeState(BaseStateEnum.Idle));
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