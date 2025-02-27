using Cysharp.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// タイトル画面の遷移を管理する
/// </summary>
public class TitleFlow
{
    private Button _button;

    public TitleFlow(Button button)
    {
        _button = button;
        _button.onClick.AddListener(GameStartButton);
    }
    
    /// <summary>
    /// ゲーム開始ボタンを押したときの処理
    /// </summary>
    private async void GameStartButton()
    {
        UIManager.Instance?.FadeOut();

        await UniTask.Delay(800);
        
        //暗転中
        UIManager.Instance?.HideStartPanel();
        SkinManager.Instance?.ChangeSkin(0);
        UIManager.Instance?.HideRightUI();
        UIManager.Instance?.HideFirstText();
        UIManager.Instance?.HideStartText();
        
        await UniTask.Delay(700);
        
        UIManager.Instance?.FadeIn();
        GameManager.Instance.SetGameState(GameState.Movie); // 開始演出へ
    }
}
