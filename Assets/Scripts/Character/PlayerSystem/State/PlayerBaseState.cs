using System;
using PlayerSystem.Input;
using PlayerSystem.State;

/// <summary>
/// プレイヤーのベースステートにインターフェースを追加したもの
/// </summary>
public abstract class PlayerBaseState<TEnum> : BaseState<TEnum> where TEnum : Enum
{
    protected readonly IPlayerStateMachine PlayerStateMachine;

    protected PlayerBaseState(IPlayerStateMachine stateMachine) : base((BaseStateMachine<TEnum, IState>)stateMachine)
    {
        PlayerStateMachine = stateMachine;
    }

    protected PlayerInputProcessor InputProcessor => PlayerStateMachine.InputProcessor;
    protected PlayerBlackBoard BlackBoard => PlayerStateMachine.BlackBoard;
    protected PlayerActionHandler ActionHandler => PlayerStateMachine.ActionHandler;
}