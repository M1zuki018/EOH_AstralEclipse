using PlayerSystem.Input;
using PlayerSystem.State.Base;

namespace PlayerSystem.State
{
    /// <summary>
    /// プレイヤーの状態を管理し、ステートマシンの更新を行う
    /// </summary>
    public class PlayerStateMachine : BaseStateMachine<BaseStateEnum, IState>, IPlayerStateMachine
    {
        private readonly PlayerInputProcessor _inputProcessor;
        private readonly PlayerBlackBoard _blackboard;
        private readonly PlayerActionHandler _actionHandler;
        
        public PlayerInputProcessor InputProcessor => _inputProcessor;
        public PlayerBlackBoard BlackBoard => _blackboard;
        public PlayerActionHandler ActionHandler => _actionHandler;
        
        /// <summary>
        /// 初期化（enumとIStateのペアを辞書に登録する）
        /// </summary>
        public PlayerStateMachine(PlayerInputProcessor inputProcessor, PlayerBlackBoard blackboard, PlayerActionHandler actionHandler) 
        {
            _inputProcessor = inputProcessor;
            _blackboard = blackboard;
            _actionHandler = actionHandler;
            
            States[BaseStateEnum.Idle] = new IdleState(this);
            States[BaseStateEnum.Walk] = new WalkState(this);
            States[BaseStateEnum.Run] = new RunState(this);
            States[BaseStateEnum.Jump] = new JumpState(this);
            States[BaseStateEnum.Step] = new StepState(this);
            States[BaseStateEnum.Guard] = new GuardState(this);
            States[BaseStateEnum.Parry] = new ParryState(this);
            States[BaseStateEnum.Attack] = new AttackState(this);
            States[BaseStateEnum.Counter] = new CounterState(this);
            States[BaseStateEnum.Hit] = new HitState(this);
            States[BaseStateEnum.Dead] = new DeadState(this);
            States[BaseStateEnum.Performance] = new PerformanceState(this);
        }
    }
}