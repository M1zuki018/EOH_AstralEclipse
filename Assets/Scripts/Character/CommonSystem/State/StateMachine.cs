using Cysharp.Threading.Tasks;
using PlayerSystem.State;

/// <summary>
/// ステートマシンを管理するクラス
/// </summary>
public class StateMachine
{
    private IState _currentState; // 現在のステート
    private readonly StateMachineManager _smm;

    public StateMachine(StateMachineManager smm)
    {
        _smm = smm;
    }

    /// <summary>
    /// ステートを変更する
    /// </summary>
    public async UniTask TransitionToState(IState newState)
    {
        if (_currentState != null)
        {
            await _currentState.Exit();
        }

        _currentState = newState;
        await _currentState.Enter();
    }

    /// <summary>
    /// ステートのExecuteを呼び出し続ける
    /// </summary>
    public async UniTask Update()
    {
        if (_currentState != null)
        {
            await _currentState.Execute();
        }
    }
}