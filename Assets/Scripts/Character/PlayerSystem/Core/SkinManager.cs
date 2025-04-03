using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Observable = UniRx.Observable;

public class SkinManager : ViewBase
{
    public static SkinManager Instance;
    
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private List<SkinPatterns> _skinPatterns;

    public override UniTask OnAwake()
    {
        Instance = this;
        if (_skinnedMeshRenderer == null) _skinnedMeshRenderer = GameObject.Find("Face").GetComponent<SkinnedMeshRenderer>();
        return base.OnAwake();
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

    /// <summary>
    /// 表情を変更する
    /// </summary>
    public void SkinTween(int index, float endValue, float duration)
    {
        float startWeight = _skinnedMeshRenderer.GetBlendShapeWeight(index); //現在のウェイト値を取得
        float elapsedTime = 0f; //経過時間
        
        Observable
            .EveryUpdate()
            .TakeWhile(_ => elapsedTime < duration)
            .Subscribe(_ =>
            {
                float t = Mathf.Clamp01(elapsedTime / duration); // 経過割合を計算（0～1の範囲）
                float currentWeight = Mathf.Lerp(startWeight, endValue, t); // 線形補間でウェイトを計算
                _skinnedMeshRenderer.SetBlendShapeWeight(index, currentWeight); // BlendShape を更新
            },
            () =>
            {
                // 完了時に確実に最終値を設定
                _skinnedMeshRenderer.SetBlendShapeWeight(index, endValue);
            })
            .AddTo(this); // メモリリーク防止のため AddTo を使用
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