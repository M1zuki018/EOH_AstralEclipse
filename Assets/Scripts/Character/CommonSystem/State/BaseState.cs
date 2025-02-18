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

    public virtual UniTask Enter() => UniTask.CompletedTask;
    public virtual UniTask Execute() => UniTask.CompletedTask;
    public virtual UniTask Exit() => UniTask.CompletedTask;
}