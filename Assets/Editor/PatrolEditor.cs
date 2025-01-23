using UnityEditor;
using UnityEngine;

/// <summary>
/// 巡回地点を手動で追加・削除・移動できるようにする
/// </summary>
[CustomEditor(typeof(PatrolRoute))]
public class PatrolEditor : Editor
{
    private void OnSceneGUI()
    {
        PatrolRoute patrolRoute = (PatrolRoute)target;
        
        if (patrolRoute.Waypoints == null || patrolRoute.Waypoints.Count == 0) return;
        
        Undo.RecordObject(patrolRoute, "Move Patrol Point"); //Undoをサポート

        for (int i = 0; i < patrolRoute.Waypoints.Count; i++)
        {
            //ハンドルで位置を変更
            Vector3 newPos = Handles.PositionHandle(patrolRoute.Waypoints[i], Quaternion.identity);

            if (newPos != patrolRoute.Waypoints[i]) //位置が変わったら更新
            {
                patrolRoute.Waypoints[i] = newPos;
                EditorUtility.SetDirty(patrolRoute); // ScriptableObjectを保存
            }
        }
    }
}