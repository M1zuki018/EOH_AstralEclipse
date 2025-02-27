using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 演出関連のクラスを一元管理するクラス
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    // GameFlowManagerで一元管理
    // ほかの Flow クラスでは MonoBehavior は継承しないこと

    #region 登録

    [Header("タイトル")]
    [SerializeField] private Button _startButton;

    #endregion
    
    private TitleFlow _title;

    private void OnEnable()
    {
        _title = new TitleFlow(_startButton);
        
        GameManager.Instance.SetGameState(GameState.Title);
    }

    private void Bind()
    {
    }
}
