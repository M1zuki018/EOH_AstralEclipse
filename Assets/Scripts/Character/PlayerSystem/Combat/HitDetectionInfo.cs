using System;
using UnityEngine;

/// <summary>
/// ヒット判定の情報を管理するクラス
/// </summary>
[Serializable]
public struct HitDetectionInfo
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Size;
    public float Duration;
}
