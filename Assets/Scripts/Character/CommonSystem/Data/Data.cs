using System;
using UnityEngine;

/// <summary>
/// 検出のタイミングと範囲
/// </summary>
[Serializable]
public struct Data
{
    /// <summary>検出範囲</summary>
    public Vector2 Range;

    /// <summary>衝突検出用の衝突データ</summary>
    [NonReorderable] public CollisionData[] Collisions;

    /// <summary>検出範囲内かどうかを確認</summary>
    /// <param name="frame">現在のフレーム</param>
    /// <returns>検出範囲内の場合はtrue</returns>
    public bool IsInRange(float frame)
    {
        return frame >= Range.x && frame <= Range.y;
    }
}