using System;
using UnityEngine;

/// <summary>
/// エフェクトの位置・回転のデータを持つ構造体
/// </summary>
[Serializable]
public struct EffectPositionInfo
{
    public Vector3 Position;
    public Quaternion Rotation;
}
