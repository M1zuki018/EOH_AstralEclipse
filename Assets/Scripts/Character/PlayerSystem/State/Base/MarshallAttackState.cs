
using Cysharp.Threading.Tasks;

namespace PlayerSystem.State.Base 
{
    /// <summary>
    /// MarshallAttack状態 
    /// </summary>
    public class MarshallAttackState : PlayerBaseState<BaseStateEnum> 
    {
        public MarshallAttackState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            BlackBoard.ApplyGravity = true;
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.MarshallAttack)
            {
                if (BlackBoard.AttackFinishedTrigger)
                {
                    BlackBoard.AttackFinishedTrigger = false;
                    StateMachine.ChangeState(BaseStateEnum.Idle);
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
            
            await UniTask.Yield();
        }
    }
}