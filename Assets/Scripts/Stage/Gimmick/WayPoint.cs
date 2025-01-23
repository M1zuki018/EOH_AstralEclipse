using UnityEngine;

/// <summary>
/// 目標地のアイコンを表示する
/// </summary>
public class Waypoint : MonoBehaviour
{
    /// <summary>
    /// アイコンの表示非表示を切り替える
    /// </summary>
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}