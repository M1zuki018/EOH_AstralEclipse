using System;
using Cysharp.Threading.Tasks;

/// <summary>
/// 各ステートのベースクラス
/// </summary>
public abstract class BaseState<TEnum> : IState where TEnum : Enum
{
    protected readonly BaseStateMachine<TEnum, IState> StateMachine;

    protected BaseState(BaseStateMachine<TEnum, IState> stateMachine)
    {
        StateMachine = stateMachine;
    }

    public UniTask Enter() => UniTask.CompletedTask;
    public UniTask Execute() => UniTask.CompletedTask;
    public UniTask Exit() => UniTask.CompletedTask;
}