using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// 対象にダメージを与える処理
/// </summary>
public class DamageHandler : MonoBehaviour
{
    /// <summary>
    /// ダメージを与える
    /// </summary>
    public void ApplyDamage(IDamageable target, int damage, GameObject attacker)
    {
        target.TakeDamage(damage, attacker);
    }
}