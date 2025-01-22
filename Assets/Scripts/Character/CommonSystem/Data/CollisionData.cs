using System;
using UnityEngine;

/// <summary>
/// 衝突検出範囲
/// </summary>
[Serializable]
public struct CollisionData
{
    /// <summary>相対座標</summary>
    public Vector3 Offset;

    /// <summary>検出ボックスの角度（オイラー角）</summary>
    public Vector3 Rotation;

    /// <summary>コライダーのサイズ。デフォルトは(1m, 1m, 1m)</summary>
    public Vector3 Scale;
}