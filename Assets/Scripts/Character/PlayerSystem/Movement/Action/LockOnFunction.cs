using System.Collections.Generic;
using PlayerSystem.ActionFunction;
using UnityEngine;

/// <summary>
/// ロックオン機能
/// </summary>
public class LockOnFunction : MonoBehaviour, ILockOnable
{
    [SerializeField] private Camera _camera; //Unityのカメラ
    [SerializeField] private float _lockOnRadius = 0.5f; //カメラ中心からのロックオン範囲
    [SerializeField] private float _detectionRange = 50f; //敵を検出する範囲
    
    public List<Transform> _inRangeEnemies = new List<Transform>(); //視界内の敵リスト
    private Transform _lockedOnEnemy; //ロックオン中の敵

    private void Update()
    {
        UpdateEnemiesInRange();
    }

    /// <summary>
    /// 視界内の敵リストを更新する
    /// </summary>
    private void UpdateEnemiesInRange()
    {
        _inRangeEnemies.Clear();
        
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in allEnemies)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, enemy.transform.position);
            
            //プレイヤーから一定距離内の敵をリストに追加
            if (distanceToPlayer <= _detectionRange)
            {
                _inRangeEnemies.Add(enemy.transform);
            }
        }
    }

    /// <summary>
    /// ロックオン機能
    /// </summary>
    public void LockOn()
    {
        _lockedOnEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var enemy in _inRangeEnemies)
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(enemy.position);
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            float distanceFromCenter = Vector2.Distance(screenPoint, screenCenter);

            if (distanceFromCenter <= _lockOnRadius * Screen.height) //スクリーン座標で比較
            {
                _lockedOnEnemy = enemy;
                break; //中心近くの敵が見つかったら処理を中断する
            }
            
            //中心以外の敵が見つからなかった場合は、最も近い敵を確認する
            float worldDistance = Vector3.Distance(transform.position, enemy.position);
            if (worldDistance < closestDistance)
            {
                closestDistance = worldDistance;
                _lockedOnEnemy = enemy;
            }
        }

        if (_lockedOnEnemy != null)
        {
            Debug.Log("ロックオン中" + _lockedOnEnemy.name);
        }
        else
        {
            Debug.Log("敵が見つかりませんでした");
        }
    }
}
