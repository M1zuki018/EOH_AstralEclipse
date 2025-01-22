using System;

namespace PlayerSystem.Fight
{
    /// <summary>
    /// ダメージの計算を行うクラス
    /// TODO:クリティカルや属性補正などを行う場合はこのクラスに実装を書く
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>
        /// ダメージ計算を行う
        /// </summary>
        /// <param name="baseAttack">攻撃力</param>
        /// <param name="defense">防御力</param>
        /// <returns>最終的なダメージ量</returns>
        public static int CalculateDamage(int baseAttack, int defense)
        {
            int finalDamage = baseAttack - defense;
            return Math.Max(finalDamage, 1); // 最低でも1ダメージは与える
        }
    }
}