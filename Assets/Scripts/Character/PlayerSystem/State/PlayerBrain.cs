using System;
using PlayerSystem.Fight;
using UnityEngine;

public class PlayerBrain : MonoBehaviour, IDamageable
{
    private UIManager _uiManager;
    private Health _health;
    void Start()
    {
        _health.OnDamaged += HandleDamage; //ダメージイベントを追加
        _health.OnDeath += HandleDeath; //死亡イベントを追加
    }

    private void OnDestroy()
    {
        _health.OnDamaged -= HandleDamage; //ダメージイベントを解除
        _health.OnDeath -= HandleDeath; //死亡イベントを解除
    }

    /// <summary>
    /// ダメージを受けた時の処理
    /// </summary>
    public void TakeDamage(int damage, GameObject attacker)
    {
        throw new System.NotImplementedException();
    }
    
    /// <summary>
    /// ダメージを受けたときの処理
    /// </summary>
    private void HandleDamage(int damage, GameObject attacker)
    {
        _uiManager.UpdatePlayerHP(damage);
        //TODO:エネミーHPバーの管理方法を考える
    }
    
    /// <summary>
    /// 死亡時の処理
    /// </summary>
    private void HandleDeath(GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}に倒された！");
        //TODO:死亡エフェクト等の処理
    }
}
