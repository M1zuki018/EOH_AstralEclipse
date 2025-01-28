using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;
    
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private List<SkinPatterns> _skinPatterns;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// 表情を変更する
    /// </summary>
    public void ChangeSkin(int index)
    {
        var skinPattern = _skinPatterns[index];
        foreach (var skinData in skinPattern.SkinPattern)
        {
            _skinnedMeshRenderer.SetBlendShapeWeight(skinData.SkinIndex, skinData.Weight);
        }
        
    }
}

/// <summary>
/// 表情を管理する構造体
/// </summary>
[Serializable]
public class SkinPatterns
{
    public List<SkinData> SkinPattern;
}

[Serializable]
public struct SkinData
{
    public int SkinIndex;
    public float Weight;
}