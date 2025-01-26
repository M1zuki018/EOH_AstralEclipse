using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 臨戦態勢を判定するクラス
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class ReadyForBattleChecker : MonoBehaviour
{
    [SerializeField] float _radius;
    [SerializeField] string _tag;
    private SphereCollider _collider;
    public bool ReadyForBattle { get; private set; }
    public EnemyBrain EnemyBrain{get; private set;}
    public Dictionary<EnemyBrain, EnemyBrain> EnemyBrainDic = new Dictionary<EnemyBrain, EnemyBrain>();
    public event Action OnReadyForBattle; //臨戦状態になったときのイベント
    public event Action OnRescission; //臨戦状態が解除されたときのイベント

    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.radius = _radius; //半径をセットする
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_tag == "Enemy" && other.CompareTag("Player"))
        {
            ReadyForBattle = true; //タグがエネミーの場合は、プレイヤータグを検知する
            OnReadyForBattle?.Invoke();
        }
        else if (_tag == "Player" && other.CompareTag("Enemy"))
        {
            ReadyForBattle = true; //タグがプレイヤーの場合は、エネミータグを検知する
            other.TryGetComponent(out EnemyBrain brain);
            EnemyBrainDic.Add(brain, brain); //EnemyBrainを取得してセットする
            EnemyBrain = brain;
            OnReadyForBattle?.Invoke();
        }
        else
        {
            ReadyForBattle = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_tag == "Enemy" && other.CompareTag("Player"))
        {
            ReadyForBattle = false; //タグがエネミーの場合は、プレイヤータグを検知する
            OnRescission?.Invoke();
        }
        else if (_tag == "Player" && other.CompareTag("Enemy"))
        {
            other.TryGetComponent(out EnemyBrain brain);
            EnemyBrain = brain;
            ReadyForBattle = false; //タグがプレイヤーの場合は、エネミータグを検知する
            OnRescission?.Invoke();
        }
        else
        {
            ReadyForBattle = false;
        }
    }
}
