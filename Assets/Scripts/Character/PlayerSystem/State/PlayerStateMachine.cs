using PlayerSystem.State.Base;

namespace PlayerSystem.State
{
    /// <summary>
    /// プレイヤーの状態を管理し、ステートマシンの更新を行う
    /// </summary>
    public class PlayerStateMachine : BaseStateMachine<BaseStateEnum, IState>
    {
        /// <summary>
        /// 初期化（enumとIStateのペアを辞書に登録する）
        /// </summary>
        public PlayerStateMachine() 
        {
            States[BaseStateEnum.Idle] = new IdleState(this);
        }
    }
}