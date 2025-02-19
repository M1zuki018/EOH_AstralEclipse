using PlayerSystem.Input;
using PlayerSystem.State;

/// <summary>
/// ベースステートマシンにプレイヤーの要素を追加するためのインターフェース
/// </summary>
public interface IPlayerStateMachine
{
    PlayerInputProcessor InputProcessor { get; }
    PlayerBlackBoard BlackBoard { get; }
    PlayerActionHandler ActionHandler { get; }
}