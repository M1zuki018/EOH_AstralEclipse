using UnityEngine;

/// <summary>
/// 目標地のアイコンを表示する
/// </summary>
public class Waypoint : MonoBehaviour
{
    public WayPointSystem WayPointSystem{ get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WayPointSystem.NextWaypoint();
        }
    }

    /// <summary>
    /// アイコンの表示非表示を切り替える
    /// </summary>
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}