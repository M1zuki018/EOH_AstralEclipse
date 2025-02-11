using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム開始演出
/// </summary>
public class GameStartPerformance : MonoBehaviour
{
    [SerializeField] private Button _button;
    
    private void Start()
    {
        GameManager.Instance.SetGameState(GameState.Title);
        _button.onClick.AddListener(() => GameStartButton());
        GameManager.Instance.OnPlay += GameStart;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPlay -= GameStart;
    }

    /// <summary>
    /// ゲーム開始ボタンを押したときの処理
    /// </summary>
    private async void GameStartButton()
    {
        //フェードアウトしつつスタートパネルを非表示
        UIManager.Instance?.FadeOut();
        UIManager.Instance?.HideStartPanel();
        
        await UniTask.Delay(1000);
        
        GameManager.Instance.SetGameState(GameState.Playing); //ステート変更
    }

    /// <summary>
    /// ゲーム開始時の処理
    /// </summary>
    private void GameStart()
    {
        SkinManager.Instance?.ChangeSkin(0);
        CameraManager.Instance?.UseCamera(3);
        UIManager.Instance?.HideRightUI();
        UIManager.Instance?.HideFirstText();
        UIManager.Instance?.HideStartText();
        UIManager.Instance?.FadeIn();
    }
    
}
