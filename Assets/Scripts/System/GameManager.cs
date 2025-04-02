using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ゲーム全体を管理する
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    /// <summary>現在のゲームの進行状態</summary>
    private readonly ReactiveProperty<GameState> _currentGameStateProp  = new ReactiveProperty<GameState>(GameState.Title);
    public ReactiveProperty<GameState> CurrentGameStateProp => _currentGameStateProp;
    public GameState CurrentGameState => _currentGameStateProp.Value;

    public event Action OnMovie; // ムービー中
    public event Action OnPlay; // プレイ中（操作可能状態）
    public event Action OnPaused; //ポーズ
    public event Action OnGameOver; //ゲームオーバー
    public event Action OnClear; //ゲームクリア
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // ステート変更時に対応した処理を呼び出す
        _currentGameStateProp.Subscribe(state =>
        {
            Debug.Log($"<color=red>【ChangeGameState】</color>　ゲーム状態が変更されました: 現在 {state}");
            HandleStateChange(state);
        }).AddTo(this);
    }
    
    /// <summary>
    /// ステートを変更するメソッド
    /// </summary>
    public void SetGameState(GameState newState)
    {
        if (_currentGameStateProp.Value != newState)
        {
            _currentGameStateProp.Value = newState;
        }
    }

    /// <summary>
    /// 各種ステートごとに呼び出される処理
    /// </summary>
    private void HandleStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Title:
                break;
            case GameState.Movie:
                OnMovie?.Invoke();
                break;
            case GameState.Playing:
                OnPlay?.Invoke();
                break;
            case GameState.Paused:
                OnPaused?.Invoke();
                break;
            case GameState.GameOver:
                OnGameOver?.Invoke();
                break;
            case GameState.Clear:
                OnClear?.Invoke();
                break;
        }
    }
}
