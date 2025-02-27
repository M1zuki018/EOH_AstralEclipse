using System;
using PlayerSystem.State;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 演出関連のクラスを一元管理するクラス
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    // GameFlowManagerで一元管理
    // ほかの Flow クラスでは MonoBehavior は継承しないこと
    // 入力制限はステートに合わせて PlayerInputManager 側で管理

    #region フィールド

    [Header("タイトル")]
    [SerializeField] private Button _startButton;
    private TitleFlow _title;

    [Header("開始演出")] 
    [SerializeField] private PlayerBrain _playerBrain;
    private GameStartFlow _gameStart;
    
    #endregion

    
    private void Start()
    {
        _title = new TitleFlow(_startButton);
        _gameStart = new GameStartFlow(_playerBrain.BB);
        
        GameManager.Instance.SetGameState(GameState.Title);
    }

    private void OnDestroy()
    {
        _gameStart.Dispose();
    }
}
