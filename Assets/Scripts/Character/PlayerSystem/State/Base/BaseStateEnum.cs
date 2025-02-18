namespace PlayerSystem.State.Base
{
    /// <summary>
    /// プレイヤーの基本的なステートの列挙型
    /// </summary>
    public enum BaseStateEnum
    {
        Default,
        Idle, // 待機
        Walk, // 歩き状態
        Run, // 走り状態
        Jump, // ジャンプ
        Step, // 回避
        Guard, // ガード
        Parry, // パリィ
        Attack, // 攻撃
        Counter, // カウンター
        Hit, // 被ダメ
        Dead, // 死亡
        Performance, // 演出中
    }
}
