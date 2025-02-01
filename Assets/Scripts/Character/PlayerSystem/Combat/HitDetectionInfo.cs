using System;
using UnityEngine;

/// <summary>
/// ヒット判定の情報を管理するクラス
/// </summary>
[Serializable]
public struct HitDetectionInfo
{
    public Vector3 Collider;
    public float Duration;
}
