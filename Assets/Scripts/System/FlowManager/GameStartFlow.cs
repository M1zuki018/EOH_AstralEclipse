using Cysharp.Threading.Tasks;
using PlayerSystem.State;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ゲーム開始演出の演出・遷移
/// </summary>
public class GameStartFlow
{
    private PlayerBlackBoard _bb;
    private CompositeDisposable _disposables = new CompositeDisposable();
    
    public GameStartFlow(PlayerBlackBoard bb)
    {
        _bb = bb;
        GameManager.Instance.OnMovie += StartPerformance;
    }
    
    /// <summary>
    /// 開始演出中の処理
    /// </summary>
    private async void StartPerformance()
    {
        // HPスライダーの初期化
        UIManager.Instance?.InitializePlayerHP(_bb.Status.MaxHP, _bb.CurrentHP);
        UIManager.Instance?.InitializePlayerWill(_bb.Status.Will, _bb.CurrentWill);

        await UniTask.Delay(2700);
        
        //操作開始
        CameraManager.Instance?.UseCamera(0);
        
        await UniTask.Delay(1200);
        
        UIManager.Instance?.ShowFirstText(); //最初のクエスト説明を表示
        _bb.MoveActions[0].action.Enable(); //有効化
        
        // ボタンが押されたら入力を有効化
        Observable.FromEvent<InputAction.CallbackContext>(
                h => _bb.MoveActions[0].action.performed += h,
                h => _bb.MoveActions[0].action.performed -= h)
            .Take(1) // 最初の1回だけ
            .Subscribe(GameStart)
            .AddTo(_disposables);
    }

    private async void GameStart(InputAction.CallbackContext context)
    {
        Debug.Log("Game started");
        AudioManager.Instance?.PlaySE(9);
        UIManager.Instance?.ShowStartText();
        UIManager.Instance?.HideFirstText();
        UIManager.Instance?.ShowRightUI();
        
        await UniTask.Delay(500);
        
        UIManager.Instance.HideStartText(); //「GameStart」の文字を非表示にする
        GameManager.Instance.SetGameState(GameState.Playing); // 捜査開始
    }

    public void Dispose()
    {
        GameManager.Instance.OnPlay -= StartPerformance;
        _disposables?.Dispose();
    }
}
