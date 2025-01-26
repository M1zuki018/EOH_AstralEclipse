using System;
using UnityEngine;

/// <summary>
/// 衝突検出範囲
/// </summary>
[Serializable]
public struct CollisionData
{
    /// <summary>検出範囲</summary>
    public Vector2 Range;
    
    /// <summary>相対座標</summary>
    public Vector3 Offset;

    /// <summary>検出ボックスの角度（オイラー角）</summary>
    public Vector3 Rotation;

    /// <summary>コライダーのサイズ。デフォルトは(1m, 1m, 1m)</summary>
    public Vector3 Scale;
    
    /// <summary>検出範囲内かどうかを確認</summary>
    /// <param name="frame">現在のフレーム</param>
    /// <returns>検出範囲内の場合はtrue</returns>
    public bool IsInRange(float frame)
    {
        return frame >= Range.x && frame <= Range.y;
    }
}