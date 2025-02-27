using Cysharp.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// タイトル画面の演出・遷移
/// </summary>
public class TitleFlow
{
    private Button _button;

    public TitleFlow(Button button)
    {
        _button = button;
        _button.onClick.AddListener(GameStartButton);
        CameraManager.Instance.UseCamera(3); //プレイヤー正面のカメラを使う
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
