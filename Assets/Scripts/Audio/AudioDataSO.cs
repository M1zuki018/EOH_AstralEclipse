using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AudioClipを管理するスクリプタブルオブジェクト
/// </summary>
[CreateAssetMenu(fileName = "AudioDataSO", menuName = "Create AudioDataSO")]
public class AudioDataSO : ScriptableObject
{
    public AudioType AudioType;
    public List<ClipData> Clips = new List<ClipData>();
}

/// <summary>
/// クリップの構造体
/// </summary>
[Serializable]
public struct ClipData
{
    public AudioClip Clip;
    public float Volume;
    public bool Loop;
}

/// <summary>
/// 音源の種類
/// </summary>
public enum AudioType
{
    BGM,
    SE,
    Voice,
    Environment,
}
