namespace PlayerSystem.Movement
{

    /// <summary>
    /// ジャンプを担当するクラスが実装する
    /// </summary>
    public interface IJumpable
    {
        /// <summary>飛び上がるときの処理</summary>
        void Jump();

        /// <summary>ジャンプ中に行われる処理</summary>
        void Jumping();
    }
}
