using UnityEngine;

/// <summary>
/// ターゲットの座標を取得するインターフェース
/// </summary>
public interface IMatchTarget
{
    Vector3 TargetPosition { get; }
}