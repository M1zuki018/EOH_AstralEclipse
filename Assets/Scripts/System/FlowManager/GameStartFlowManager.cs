using Cysharp.Threading.Tasks;
using PlayerSystem.State;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ゲーム開始演出の流れを管理する
/// </summary>
public class GameStartFlowManager : MonoBehaviour
{
    [SerializeField, HighlightIfNull] private PlayerInput _playerInput;
    private PlayerBlackBoard _bb;

    private void Start()
    {
        _bb = _playerInput.GetComponent<PlayerBrain>().BB;
        
        _playerInput.DeactivateInput(); //入力を受け付けない
        
        GameManager.Instance.OnPlay += StartPerformance;
        CameraManager.Instance.UseCamera(3); //プレイヤー正面のカメラを使う
    }
    
    /// <summary>
    /// 開始演出中の処理
    /// </summary>
    private async void StartPerformance()
    {
        if (!_bb.DebugMode)
        {
            _playerInput.DeactivateInput();
            UIManager.Instance?.InitializePlayerHP(_bb.Status.MaxHP, _bb.CurrentHP);   

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
                .AddTo(this);
        }
    }

    private async void GameStart(InputAction.CallbackContext context)
    {
        Debug.Log("Game started");
        AudioManager.Instance?.PlaySE(9);
        UIManager.Instance?.ShowStartText();
        UIManager.Instance?.HideFirstText();
        UIManager.Instance?.ShowRightUI();
        _playerInput.ActivateInput();
        
        await UniTask.Delay(500);
        
        UIManager.Instance.HideStartText(); //「GameStart」の文字を非表示にする
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPlay -= StartPerformance;
    }
}
