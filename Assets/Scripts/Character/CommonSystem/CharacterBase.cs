using UnityEngine;
using PlayerSystem.Fight;

/// <summary>
/// 全キャラクターの基底クラス
/// </summary>
public abstract class CharacterBase : MonoBehaviour, IDamageable
{
    protected IHealth _health; //体力の管理
    protected UIManager _uiManager; //UIの管理
    
    protected virtual void Awake() 
    { 
        _health = GetComponent<IHealth>(); 
        _uiManager = FindObjectOfType<UIManager>(); 
        _health.OnDamaged += HandleDamage; //イベント登録
        _health.OnDeath += HandleDeath;
    }

    protected virtual void OnDestroy()
    {
        _health.OnDamaged -= HandleDamage; //イベント解除
        _health.OnDeath -= HandleDeath;
    }

    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    public void TakeDamage(int damage, GameObject attacker)
    { 
        _health.TakeDamage(damage, attacker);
    }
    
    public int GetCurrentHP() => _health.CurrentHP; // HP を取得
    public int GetMaxHP() => _health.MaxHP; // 最大 HP を取得

    /// <summary>ダメージを受けた時の処理</summary>
    protected abstract void HandleDamage(int damage, GameObject attacker);
    /// <summary>死亡した時の処理</summary>
    protected abstract void HandleDeath(GameObject attacker);
}
