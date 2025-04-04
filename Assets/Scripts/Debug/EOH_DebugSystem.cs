using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// ブラッシュアップ中のデバッグシステム
/// </summary>
public class EOH_DebugSystem : ViewBase
{
    [FormerlySerializedAs("_titleSkip")]
    [Header("開始演出関連")]
    [SerializeField, Comment("開始演出をスキップする")] private bool _isTitleSkip;
    public bool IsIsTitleSkip => _isTitleSkip;
    
    [Header("ボス戦をスタートさせる")]
    [SerializeField] private PlayerBrain _playerBrain;

    public override UniTask OnStart()
    {
        StartBossBattle();
        return base.OnStart();
    }


    /// <summary>
    /// タイトルをスキップする
    /// </summary>
    public void TitleSkip()
    {
        Debug.Log("Game started");
        GameManager.Instance.SetGameState(GameState.Playing);
        
        // カメラ
        CameraManager.Instance?.UseCamera(0);
        
        // UI表示
        UIManager.Instance?.HideStartPanel();
        UIManager.Instance?.HideFirstText();
        UIManager.Instance?.HideStartText();
        UIManager.Instance?.ShowRightUI();
        
        // BGM
        AudioManager.Instance.ClipChange(AudioType.BGM, 1);
    }

    /// <summary>
    /// ボス戦を始める
    /// </summary>
    private void StartBossBattle()
    {
        _playerBrain.BB.IsBossBattle = true;
    }
}
