using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ヒットストップ機能を提供するクラス
/// </summary>
public class HitStop : MonoBehaviour
{
    private float _initialTimeScale;

    public async void ApplyHitStop(float duration = 0.01f)
    {
        _initialTimeScale = Time.timeScale; //現在のタイムスケールを保存

        Time.timeScale = 0.1f;

        await UniTask.Delay(System.TimeSpan.FromSeconds(duration));

        Time.timeScale = _initialTimeScale;
    }
}