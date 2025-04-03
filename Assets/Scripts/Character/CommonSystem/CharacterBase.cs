using Cysharp.Threading.Tasks;
using UnityEngine;
using PlayerSystem.Fight;

/// <summary>
/// 全キャラクターの基底クラス
/// </summary>
public abstract class CharacterBase : ViewBase
{
    protected IHealth _health; //体力の管理
    
    public override UniTask OnAwake() 
    { 
        _health = GetComponent<IHealth>(); 
        _health.OnDamaged += HandleDamage; //イベント登録
        _health.OnDeath += HandleDeath;
        
        return base.OnAwake();
    }

    protected virtual void OnDestroy()
    {
        _health.OnDamaged -= HandleDamage; //イベント解除
        _health.OnDeath -= HandleDeath;
    }
    
    /// <summary>現在のHPを取得する</summary>
    public int GetCurrentHP() => _health.CurrentHP;
    
    /// <summary>最大HPを取得する</summary>
    public int GetMaxHP() => _health.MaxHP;
    
    /// <summary>現在のWillを取得する</summary>
    public int GetCurrentWill() => _health.CurrentHP;
    
    /// <summary>最大HPを取得する</summary>
    public int GetMaxWill() => _health.MaxHP;

    /// <summary>ダメージを受けた時の処理</summary>
    protected abstract void HandleDamage(int damage, GameObject attacker);
    /// <summary>死亡した時の処理</summary>
    protected abstract void HandleDeath(GameObject attacker);
}
