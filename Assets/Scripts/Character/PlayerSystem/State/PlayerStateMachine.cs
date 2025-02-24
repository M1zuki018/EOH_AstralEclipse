using PlayerSystem.Input;
using PlayerSystem.State.Base;
using UnityEngine;

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
        private readonly Animator _animator;
        
        public PlayerInputProcessor InputProcessor => _inputProcessor;
        public PlayerBlackBoard BlackBoard => _blackboard;
        public PlayerActionHandler ActionHandler => _actionHandler;
        public Animator Animator => _animator;
        
        /// <summary>
        /// 初期化（enumとIStateのペアを辞書に登録する）
        /// </summary>
        public PlayerStateMachine(PlayerInputProcessor inputProcessor, PlayerBlackBoard blackboard, PlayerActionHandler actionHandler,
        Animator animator) 
        {
            _inputProcessor = inputProcessor;
            _blackboard = blackboard;
            _actionHandler = actionHandler;
            _animator = animator;
            
            States[BaseStateEnum.Idle] = new IdleState(this);
            States[BaseStateEnum.Move] = new MoveState(this);
            States[BaseStateEnum.Jump] = new JumpState(this);
            States[BaseStateEnum.Step] = new StepState(this);
            States[BaseStateEnum.Guard] = new GuardState(this);
            States[BaseStateEnum.Parry] = new ParryState(this);
            States[BaseStateEnum.Attack] = new AttackState(this, new PlayerAttackSubStateMachine(_inputProcessor, _blackboard, _actionHandler, _animator));
            States[BaseStateEnum.Skill] = new SkillState(this);
            States[BaseStateEnum.Counter] = new CounterState(this);
            States[BaseStateEnum.Hit] = new HitState(this);
            States[BaseStateEnum.GuardBreak] = new GuardBreakState(this);
            States[BaseStateEnum.Dead] = new DeadState(this);
            States[BaseStateEnum.Performance] = new PerformanceState(this);
            
            Initialize(BaseStateEnum.Idle); // 初期化処理
        }
    }
}