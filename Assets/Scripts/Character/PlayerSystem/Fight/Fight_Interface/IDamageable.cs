using UnityEngine;

namespace PlayerSystem.Fight
{
    /// <summary>
    /// ダメージを受けるもの全てが継承する
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// ダメージを受ける
        /// </summary>
        /// <param name="damage">受けるダメージ</param>
        /// <param name="attacker">攻撃する側のゲームオブジェクト</param>
        void TakeDamage(int damage, GameObject attacker);
    }
}