using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 全ての目的地アイコンを管理する
/// </summary>
public class WayPointSystem : MonoBehaviour
{
    [SerializeField] private Waypoint[] _waypoints;
    [SerializeField] private RespawnEvent _respawn;
    private int _currentIndex = 0;

    private void Start()
    {
        foreach (Waypoint waypoint in _waypoints)
        {
            waypoint.Initialize(this, _respawn);
        }
        
        UpdateWaypoints();
    }

    /// <summary>
    /// 目的地を更新する
    /// </summary>
    private void UpdateWaypoints()
    {
        for (int i = 0; i < _waypoints.Length; i++)
        {
            _waypoints[i].SetActive(i == _currentIndex);
        }
    }

    /// <summary>
    /// 次の目的地をセットする
    /// </summary>
    public void NextWaypoint()
    {
        if (_currentIndex < _waypoints.Length - 1) //次の目的地がある場合
        {
            _currentIndex++;
            UpdateWaypoints();
        }
        else if (_currentIndex == _waypoints.Length - 1) //これ以上目的地がない場合
        {
            _waypoints[_waypoints.Length - 1].SetActive(false);
        }
    }
}