using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// ダメージを受ける処理
/// </summary>
public class DamageableEntity : MonoBehaviour, IDamageable
{
    private IHealth _health;

    private void Awake()
    {
        _health = GetComponent<IHealth>();
    }
    
    /// <summary>
    /// ダメージを受ける
    /// </summary>
    public void TakeDamage(int damage, GameObject attacker)
    {
        _health?.TakeDamage(damage);
        Debug.Log($"{gameObject.name} は {attacker.name} から {damage} ダメージを受けた ");
    }
}
