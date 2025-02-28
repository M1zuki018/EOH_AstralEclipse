using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ゲームオーバー時の処理
/// </summary>
public class GameOverPerformance : MonoBehaviour
{
    [SerializeField] private Button _restryButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        _restryButton?.onClick.AddListener(() => Retry());
        _quitButton?.onClick.AddListener(() => Quit());
    }
    
    /// <summary>
    /// リトライ処理
    /// </summary>
    public async void Retry()
    {
        UIManager.Instance.FadeOut();
        
        await WaitWithoutTimeScaleImpact(1);
        
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// 諦める処理
    /// </summary>
    public async void Quit()
    {
        UIManager.Instance.FadeOut();
        
        await WaitWithoutTimeScaleImpact(1);
        
        Time.timeScale = 1;
        EditorApplication.isPlaying = false;//ゲームプレイ終了
    }
    
    /// <summary>
    /// TimeScaleに影響を受けない待機処理
    /// </summary>
    private async UniTask WaitWithoutTimeScaleImpact(float seconds)
    {
        await UniTask.Delay((int)(seconds * 1000), DelayType.UnscaledDeltaTime);
    }
}
