namespace PlayerSystem.Fight
{
    /// <summary>
    /// 攻撃に関する処理を管理します
    /// </summary>
    public interface ICombat
    {
        public int BaseAttackPower { get; }
        
        /// <summary>攻撃補正を行うクラス</summary>
        AdjustDirection AdjustDirection { get; }
        
        /// <summary>ダメージを与える処理</summary>
        public DamageHandler DamageHandler { get; }
    }
}