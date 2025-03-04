using PlayerSystem.Input;
using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// ベースステートにプレイヤーの要素を追加するためのインターフェース
/// </summary>
public interface IPlayerStateMachine
{
    PlayerInputProcessor InputProcessor { get; }
    PlayerBlackBoard BlackBoard { get; }
    PlayerActionHandler ActionHandler { get; }
}