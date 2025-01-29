using System;
using UniRx;
using UnityEngine;

/// <summary>
/// ゲーム全体を管理する
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    /// <summary>現在のゲームの進行状態</summary>
    public ReactiveProperty<GameState> CurrentGameState { get; private set; } = new ReactiveProperty<GameState>(GameState.Playing);

    public event Action OnPlay; //プレイ開始
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
        CurrentGameState.Subscribe(state =>
        {
            Debug.Log($"ゲーム状態が変更されました: {state}");
            HandleStateChange(state);
        }).AddTo(this);
    }
    
    /// <summary>
    /// ステートを変更するメソッド
    /// </summary>
    public void SetGameState(GameState newState)
    {
        if (CurrentGameState.Value != newState)
        {
            CurrentGameState.Value = newState;
        }
    }

    /// <summary>
    /// 各種ステートごとに呼び出される処理
    /// </summary>
    /// <param name="state"></param>
    private void HandleStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Title:
                OnEnterTitle();
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

    private void OnEnterTitle() => Debug.Log("タイトル画面の処理");
}
