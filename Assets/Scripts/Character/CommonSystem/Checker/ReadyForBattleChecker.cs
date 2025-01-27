using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// 臨戦態勢を判定するクラス
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class ReadyForBattleChecker : MonoBehaviour
{
    [SerializeField] float _radius; //コライダーの半径
    [SerializeField, HighlightIfNull] private LockOnFunction _lockOnFunction;
    private SphereCollider _collider;

    /// <summary>現在の臨戦状態</summary>
    public bool ReadyForBattle { get; private set; }

    /// <summary>コライダー内の敵を管理するセット</summary>
    public HashSet<EnemyBrain> EnemiesInRange = new HashSet<EnemyBrain>();

    public event Action<EnemyBrain> OnReadyForBattle; //臨戦状態になったときのイベント
    public event Action<EnemyBrain> OnRescission; //臨戦状態が解除されたときのイベント

    private void Awake()
    {
        InitializeSphereCollider();
    }

    /// <summary>
    /// SphereColliderのセットアップ
    /// </summary>
    private void InitializeSphereCollider()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _collider.radius = _radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsValidEnemy(other, out EnemyBrain enemyBrain)) //判定を行う
        {
            AddEnemy(enemyBrain);
            _lockOnFunction.LockOn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out EnemyBrain brain))
        {
            RemoveEnemy(brain);
        }
    }
    
    /// <summary>
    /// 判定のメソッド
    /// </summary>
    private bool IsValidEnemy(Collider other, out EnemyBrain enemyBrain)
    {
        enemyBrain = null;
        return other.CompareTag("Enemy") //タグがEnemyか
               && other.TryGetComponent(out enemyBrain) //EnemyBrainが取得できるか
               && !other.GetComponent<Health>().IsDead; //Enemyが死亡していないか
    }

    /// <summary>
    /// 判定内にEnemyが侵入したときの処理
    /// </summary>
    private void AddEnemy(EnemyBrain brain)
    {
        if (EnemiesInRange.Add(brain)) //ハッシュセットに新規に追加できた場合のみ以下の処理を行う
        {
            if (!ReadyForBattle) //臨戦状態ではなかったら以下の処理を行う
            {
                ReadyForBattle = true;
            }

            OnReadyForBattle?.Invoke(brain); //イベント発火（対象のEnemyBrainを渡す）
        }
    }
    
    /// <summary>
    /// 判定内からEnemyが出たときの処理
    /// </summary>
    private void RemoveEnemy(EnemyBrain brain)
    {
        if (EnemiesInRange.Remove(brain))
        {
            // 敵が全ていなくなった場合、臨戦状態を解除する
            if (EnemiesInRange.Count == 0)
            {
                ReadyForBattle = false;
            }
            
            OnRescission?.Invoke(brain); //イベント発火（対象のEnemyBrainを渡す）
        }
    }
}
