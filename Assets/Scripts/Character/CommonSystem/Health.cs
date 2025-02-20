using System;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// すべてのエンティティのHPを管理するクラス
/// </summary>
public class Health : MonoBehaviour, IHealth
{
    public int MaxHP { get; private set; } = 100; //最大HP
    public int CurrentHP { get; private set; } //現在のHP
    public bool IsDead => CurrentHP <= 0; //HPが0以下になったら死亡する
    public event Action<int, GameObject> OnDamaged; //ダメージを受けた時のイベント
    public event Action<int, GameObject> OnHealed; //回復イベント
    public event Action<GameObject> OnDeath; //死亡イベント

    private PlayerBrain _brain;
    
    private void Awake()
    { 
        _brain = GetComponent<PlayerBrain>();

        //初期化する
        _brain.BB.CurrentWill = _brain.BB.Status.Will;
        MaxHP = _brain.BB.Status.MaxHP;
        CurrentHP = MaxHP;
    }
    
    /// <summary>
    /// ダメージを受ける処理の実装
    /// </summary>
    public void TakeDamage(int amount, GameObject attacker)
    {
        if(IsDead) return; //死亡状態ならこれ以降の処理は行わない
        
        if(attacker.tag == this.tag) return;

        if(_brain.BB.IsSteping) return; //ステップ中の場合、ダメージを受けない

        // ガード中はWillを削る
        if (_brain.BB.IsGuarding)
        {
            _brain.BB.CurrentWill -= amount;
            return;
        }
        
        CurrentHP -= amount;
        OnDamaged?.Invoke(amount, attacker); //ダメージイベント発火

        if (IsDead) //死亡判定
        {
            OnDeath?.Invoke(attacker); //死亡イベント発火
        }
    }

    /// <summary>
    /// 回復処理
    /// </summary>
    public void Heal(int amount, GameObject healer)
    {
        if(IsDead) return; //死亡状態ならこれ以降の処理は行わない
        
        CurrentHP += amount;
        OnHealed?.Invoke(amount, healer);
    }
}
