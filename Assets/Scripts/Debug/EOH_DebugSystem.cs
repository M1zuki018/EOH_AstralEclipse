using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ブラッシュアップ中のデバッグシステム
/// </summary>
public class EOH_DebugSystem : MonoBehaviour
{
    [SerializeField, Comment("開始演出をスキップする")] private bool _titleSkip;

    private void Start()
    {
        if(_titleSkip) TitleSkip();
    }

    /// <summary>
    /// タイトルをスキップする
    /// </summary>
    private void TitleSkip()
    {
        GameManager.Instance.SetGameState(GameState.Playing);
    }
}
