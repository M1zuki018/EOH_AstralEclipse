using System;
using System.Collections.Generic;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// 臨戦態勢を判定するクラス
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class ReadyForBattleChecker : MonoBehaviour
{
    [SerializeField] float _radius; //コライダーの半径
    private SphereCollider _collider;

    /// <summary>現在の臨戦状態</summary>
    public bool ReadyForBattle { get; private set; }

    /// <summary>コライダー内の敵のEnemyBrainを保持しておくディクショナリ―</summary>
    public Dictionary<EnemyBrain, EnemyBrain> EnemyBrainDic = new Dictionary<EnemyBrain, EnemyBrain>();

    public event Action<EnemyBrain> OnReadyForBattle; //臨戦状態になったときのイベント
    public event Action<EnemyBrain> OnRescission; //臨戦状態が解除されたときのイベント

    private void Start()
    {
        //SphereColliderのセットアップ
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _collider.radius = _radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && other.TryGetComponent(out EnemyBrain brain))
        {
            Health health = other.GetComponent<Health>();
            if(health.IsDead) return; //死んでいたら以降の処理は行わない
            
            if (!EnemyBrainDic.ContainsKey(brain))
            {
                EnemyBrainDic.Add(brain, brain); //まだ登録されていなかったら、取得したEnemyBrainをセットする

                if (!ReadyForBattle) //臨戦状態ではなかったら以下の処理を行う
                {
                    ReadyForBattle = true;
                }

                OnReadyForBattle?.Invoke(brain); //イベント発火（対象のEnemyBrainを渡す）
            }
        }
    }

private void OnTriggerExit(Collider other)
    {
        // EnemyBrainを取得し、存在する場合は削除
        if (other.TryGetComponent(out EnemyBrain brain) && EnemyBrainDic.ContainsKey(brain))
        {
            EnemyBrainDic.Remove(brain);

            // 敵が全ていなくなった場合に臨戦状態を解除
            if (EnemyBrainDic.Count == 0 && ReadyForBattle)
            {
                ReadyForBattle = false;
            }
            
            OnRescission?.Invoke(brain); //イベント発火（対象のEnemyBrainを渡す）
        }
    }
}
