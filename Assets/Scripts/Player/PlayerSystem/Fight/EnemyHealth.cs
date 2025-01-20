using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// 敵のHPを管理するクラス
/// </summary>
public class EnemyHealth : MonoBehaviour, IHealth
{
    public int MaxHP { get; private set; } //最大HP
    public int CurrentHP { get; private set; } //現在のHP
    public bool IsDead => CurrentHP <= 0; //HPが0以下になったら死亡する

    /// <summary>
    /// 初期化
    /// </summary>
    public EnemyHealth(int maxHP)
    {
        MaxHP = maxHP;
        CurrentHP = MaxHP;
    }
    
    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    public void TakeDamage(int amount)
    {
        if(IsDead) return; //死亡状態ならこれ以降の処理は行わない
        
        CurrentHP -= amount;

        if (IsDead)
        {
            Die(); //死亡したら死亡処理を行う
        }
    }

    /// <summary>
    /// 回復処理
    /// </summary>
    public void Heal(int amount)
    {
        if(IsDead) return; //死亡状態ならこれ以降の処理は行わない
        
        CurrentHP += amount;
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    public void Die()
    {
        Debug.Log("死んでしまった！！！！");
    }
}
