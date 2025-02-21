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
        
        Initialize(AttackStateEnum.NormalAttack1);
    }
}
