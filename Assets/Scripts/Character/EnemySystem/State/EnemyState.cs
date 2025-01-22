namespace Enemy.State
{
    /// <summary>
    /// 敵の状態
    /// </summary>
    public enum EnemyState
    {
        Idle, //待機状態
        Chase, //追跡状態
        Attack, //攻撃状態
        Dead //死亡状態
    }

}
