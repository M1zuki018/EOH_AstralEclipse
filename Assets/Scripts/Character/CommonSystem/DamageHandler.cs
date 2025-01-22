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
    /// <param name="target">対象のキャラクター。IDamageableを継承している必要がある</param>
    /// <param name="baseDamage">攻撃力</param>
    /// <param name="defense">防御力</param>
    /// <param name="attacker">攻撃するキャラクター</param>
    public void ApplyDamage(IDamageable target, int baseDamage, int defense, GameObject attacker)
    {
        int damage = DamageCalculator.CalculateDamage(baseDamage, defense); //ダメージ計算を行う
        target.TakeDamage(damage, attacker); //計算後のダメージをターゲットは受ける
    }
}