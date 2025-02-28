namespace PlayerSystem.State.Base
{
    /// <summary>
    /// プレイヤーの基本的なステートの列挙型
    /// </summary>
    public enum BaseStateEnum
    {
        Default,
        Idle, // 待機
        Move, // 移動状態
        Jump, // ジャンプ
        Step, // 回避
        Guard, // ガード
        Parry, // パリィ
        Attack, // 攻撃
        Skill, // スキル発動状態
        Hit, // 被ダメ
        GuardBreak, // ガードブレイク
        Dead, // 死亡
        Performance, // 演出中
    }
}
