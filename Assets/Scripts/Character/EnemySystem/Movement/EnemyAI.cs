using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField, HighlightIfNull] private Transform[] _patrolPoints;
    private int _currentPoint = 0;
    private EnemyMovement _enemyMovement;

    
    private void Start()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
    }

    /*
    private void Update()
    {
        if (!_enemyMovement.Agent.pathPending && _enemyMovement.Agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }
    */

    /// <summary>
    /// 次の巡回地点へ向かう
    /// </summary>
    public void GoToNextPoint()
    {
        if (!_enemyMovement.Agent.pathPending && _enemyMovement.Agent.remainingDistance < 0.5f)
        {
            if (_patrolPoints.Length == 0) return; //巡回地点の登録がゼロの場合、以降の処理を行わない

            _enemyMovement.Agent.destination = _patrolPoints[_currentPoint].position;
            _currentPoint = (_currentPoint + 1) % _patrolPoints.Length;
        }
    }
}