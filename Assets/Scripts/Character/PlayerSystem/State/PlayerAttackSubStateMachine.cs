using PlayerSystem.Input;
using PlayerSystem.State;
using PlayerSystem.State.Attack;

/// <summary>
/// プレイヤーのAttackステート用のサブステートマシン
/// </summary>
public class PlayerAttackSubStateMachine : BaseStateMachine<AttackStateEnum, IState>, IPlayerStateMachine
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
    public PlayerAttackSubStateMachine(PlayerInputProcessor inputProcessor, PlayerBlackBoard blackboard,
        PlayerActionHandler actionHandler)
    {
        _inputProcessor = inputProcessor;
        _blackboard = blackboard;
        _actionHandler = actionHandler;

        States[AttackStateEnum.Default] = new DefaultState(this);
        States[AttackStateEnum.NormalAttack1] = new NormalAttack1State(this);
        States[AttackStateEnum.NormalAttack2] = new NormalAttack2State(this);
        States[AttackStateEnum.NormalAttack3] = new NormalAttack3State(this);
        States[AttackStateEnum.NormalAttack4] = new NormalAttack4State(this);
        States[AttackStateEnum.NormalAttack5] = new NormalAttack5State(this);
        States[AttackStateEnum.AirAttack1] = new AirAttack1State(this);
        States[AttackStateEnum.AirAttack2] = new AirAttack2State(this);
        States[AttackStateEnum.AirAttack3] = new AirAttack3State(this);
        States[AttackStateEnum.AirAttack4] = new AirAttack4State(this);
        States[AttackStateEnum.MartialAttack1] = new MartialAttack1State(this);
        States[AttackStateEnum.MartialAttack2] = new MartialAttack2State(this);
        States[AttackStateEnum.MartialAttack3] = new MartialAttack3State(this);
        States[AttackStateEnum.MartialAttack4] = new MartialAttack4State(this);
        States[AttackStateEnum.MartialAttack5] = new MartialAttack5State(this);
        States[AttackStateEnum.AirMartialAttack1] = new AirMartialAttack1State(this);
        States[AttackStateEnum.AirMartialAttack2] = new AirMartialAttack2State(this);
        States[AttackStateEnum.AirMartialAttack3] = new AirMartialAttack3State(this);
        States[AttackStateEnum.AirMartialAttack4] = new AirMartialAttack4State(this);
        States[AttackStateEnum.AirMartialAttack5] = new AirMartialAttack5State(this);
        States[AttackStateEnum.CombatJump] = new CombatJumpState(this);
        States[AttackStateEnum.ThrowSword] = new ThrowSwordState(this);
        States[AttackStateEnum.RecoverSword] = new RecoverSwordState(this);
        
        Initialize(AttackStateEnum.Default);
    }
}
