using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField, HighlightIfNull] private PatrolRoute _patrolPoints;
    private int _currentPoint = 0;
    private EnemyMovement _enemyMovement;

    
    private void Start()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        GoToNextPoint();
    }

    /// <summary>
    /// 次の巡回地点へ向かう
    /// </summary>
    public void GoToNextPoint()
    {
        if (!_enemyMovement.Agent.pathPending && _enemyMovement.Agent.remainingDistance < 0.5f)
        {
            if (_patrolPoints.Waypoints.Count == 0) return; //巡回地点の登録がゼロの場合、以降の処理を行わない

            _currentPoint = (_currentPoint + 1) % _patrolPoints.Waypoints.Count;
            _enemyMovement.Agent.SetDestination(_patrolPoints.Waypoints[_currentPoint]);
        }
    }
    
    //Gizmos を使って巡回地点をエディタ上で可視化
    private void OnDrawGizmos()
    {
        if (_patrolPoints.Waypoints == null || _patrolPoints.Waypoints.Count == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < _patrolPoints.Waypoints.Count; i++)
        {
            Gizmos.DrawSphere(_patrolPoints.Waypoints[i], 0.5f); // 巡回地点を表示
            if (i < _patrolPoints.Waypoints.Count - 1)
            {
                Gizmos.DrawLine(_patrolPoints.Waypoints[i], _patrolPoints.Waypoints[i + 1]); // 巡回ルート
            }
        }
    }
}