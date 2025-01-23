using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵AIの巡回ルートを管理するスクリプタブルオブジェクト
/// </summary>
[CreateAssetMenu(fileName = "NewPatrolRoute", menuName = "Create PatrolRoute")]
public class PatrolRoute : ScriptableObject
{
    public List<Vector3> Waypoints = new List<Vector3>(); // 巡回地点リスト
    
    private void OnEnable()
    {
        if (Waypoints == null)
        {
            Waypoints = new List<Vector3>();
        }
    }
}