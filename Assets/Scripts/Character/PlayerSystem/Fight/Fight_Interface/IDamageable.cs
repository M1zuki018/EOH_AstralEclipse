using UnityEngine;

namespace PlayerSystem.Fight
{
    /// <summary>
    /// 攻撃対象に対するダメージ処理を定義する
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int damage, GameObject attacker);
    }
}