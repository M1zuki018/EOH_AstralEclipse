using UnityEngine;
using PlayerSystem.Fight;

/// <summary>
/// 全キャラクターの基底クラス
/// </summary>
public abstract class CharacterBase : MonoBehaviour, IDamageable
{ 
    [SerializeField] protected int _defense; // 防御力
    protected Health _health;
    protected UIManager _uiManager;
    
    protected virtual void Awake() 
    { 
        _health = GetComponent<Health>(); 
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

    /// <summary>ダメージを受けた時の処理</summary>
    protected abstract void HandleDamage(int damage, GameObject attacker);
    /// <summary>死亡した時の処理</summary>
    protected abstract void HandleDeath(GameObject attacker);
}
