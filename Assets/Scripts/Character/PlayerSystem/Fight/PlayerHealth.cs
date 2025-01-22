using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// プレイヤーのHPを管理するクラス
/// </summary>
public class PlayerHealth : MonoBehaviour, IHealth
{
    public int MaxHP { get; private set; } = 100; //最大HP
    public int CurrentHP { get; private set; } //現在のHP
    public bool IsDead => CurrentHP <= 0; //HPが0以下になったら死亡する
    public UIManager UIManager;

    private void Start()
    {
        CurrentHP = MaxHP; //HPを初期化する
        UIManager.InitializePlayerHP(MaxHP, CurrentHP); //スライダーを初期化する
    }
    
    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    public void TakeDamage(int amount)
    {
        if(IsDead) return; //死亡状態ならこれ以降の処理は行わない
        
        CurrentHP -= amount;
        UIManager.UpdatePlayerHP(CurrentHP);

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
        UIManager.UpdatePlayerHP(CurrentHP);
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    public void Die()
    {
        Debug.Log("死んでしまった！！！！");
    }
}
