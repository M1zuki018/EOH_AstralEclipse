namespace PlayerSystem.State.Attack
{
    /// <summary>
    /// プレイヤーの攻撃パターンの列挙型
    /// </summary>
    public enum AttackStateEnum
    {
        Default,
        NormalAttack1,
        NormalAttack2,
        NormalAttack3,
        NormalAttack4,
        NormalAttack5,
        AirAttack1,
        AirAttack2,
        AirAttack3,
        AirAttack4,
        MartialAttack1,
        MartialAttack2,
        MartialAttack3,
        MartialAttack4,
        MartialAttack5,
        AirMartialAttack1,
        AirMartialAttack2,
        AirMartialAttack3,
        AirMartialAttack4,
        AirMartialAttack5,
        CombatJump,
        ThrowSword,
        RecoverSword,
    }
}