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
            States[BaseStateEnum.Move] = new MoveState(this);
            States[BaseStateEnum.Jump] = new JumpState(this);
            States[BaseStateEnum.Step] = new StepState(this);
            States[BaseStateEnum.Guard] = new GuardState(this);
            States[BaseStateEnum.Parry] = new ParryState(this);
            States[BaseStateEnum.GuardBreak] = new GuardBreakState(this);
            States[BaseStateEnum.NormalAttack] = new NormalAttackState(this);
            States[BaseStateEnum.AirAttack] = new AirAttackState(this);
            States[BaseStateEnum.MarshallAttack] = new MarshallAttackState(this);
            States[BaseStateEnum.AirMarshallAttack] = new AirMarshallAttackState(this);
            States[BaseStateEnum.Skill] = new SkillState(this);
            States[BaseStateEnum.Hit] = new HitState(this); // 未作成
            States[BaseStateEnum.Dead] = new DeadState(this); // 未作成
            
            Initialize(BaseStateEnum.Idle); // 初期化処理
        }
    }
}