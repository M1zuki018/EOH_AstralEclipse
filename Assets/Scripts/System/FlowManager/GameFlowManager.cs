using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 演出関連のクラスを一元管理するクラス
/// </summary>
public class GameFlowManager : ViewBase
{
    // GameFlowManagerで一元管理
    // ほかの Flow クラスでは MonoBehavior は継承しないこと
    // 入力制限はステートに合わせて PlayerInputManager 側で管理

    #region フィールド

    [SerializeField] private EOH_DebugSystem _debugSystem;

    [Header("タイトル")]
    [SerializeField] private Button _startButton;

    [Header("開始演出")] 
    [SerializeField] private PlayerBrain _playerBrain;
    
    // 各種フロー
    private TitleFlow _title;
    private GameStartFlow _gameStart;
    private GameClearFlow _gameClear;
    
    #endregion
    
    public override UniTask OnStart()
    {
        _title = new TitleFlow(_startButton);
        _gameStart = new GameStartFlow(_playerBrain.BB);
        _gameClear = new GameClearFlow();
        
        _gameClear.Bind();
        
        // デバッグシステムでタイトルスキップが選択されていなければTitleフローから再生
        if (!_debugSystem.IsIsTitleSkip) GameManager.Instance.SetGameState(GameState.Title);
        else _debugSystem.TitleSkip();
        
        return base.OnStart();
    }

    private void OnDestroy()
    {
        _gameStart.Dispose();
        _gameClear.Dispose();
    }
}
