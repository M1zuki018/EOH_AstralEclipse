using System;
using System.Linq;
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
    /// <returns>検出範囲内の場合は true</returns>
    public bool IsInRange(float frame)
    {
        return frame >= Range.x && frame <= Range.y;
    }

    /// <summary>現在のフレームに基づく有効な CollisionData を取得</summary>
    /// <param name="frame">現在のフレーム</param>
    /// <returns>有効な CollisionData の配列</returns>
    public CollisionData[] GetActiveCollisions(float frame)
    {
        return Collisions.Where(collision => collision.IsInRange(frame)).ToArray();
    }
}