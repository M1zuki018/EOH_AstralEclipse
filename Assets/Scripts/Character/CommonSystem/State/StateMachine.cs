using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayerSystem.State;
using UniRx;

/// <summary>
/// ステートマシンを管理するクラス
/// </summary>
public class StateMachine
{
    private Dictionary<BaseStateEnum, IState> _states = new Dictionary<BaseStateEnum, IState>();
    private ReactiveProperty<IState> _currentState = new ReactiveProperty<IState>(); // 現在のステート
    private readonly PlayerStateMachine _smm;

    public StateMachine(PlayerStateMachine smm)
    {
        _smm = smm;
    }

}