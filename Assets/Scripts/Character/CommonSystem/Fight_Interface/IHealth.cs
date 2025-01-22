using UnityEngine;

namespace PlayerSystem.Fight
{
    /// <summary>
    /// HPの増減や現在のHP状態を管理します
    /// </summary>
    public interface IHealth
    {
        /// <summary>死亡状態</summary>
        bool IsDead { get; }

        /// <summary>ダメージを受ける</summary>
        void TakeDamage(int amount, GameObject attacker);
        
        /// <summary>回復する</summary>
        void Heal(int amount, GameObject healer);

        /// <summary>死亡処理</summary>
        void Die();

    }

}
