namespace PlayerSystem.Movement
{
    /// <summary>
    /// 歩き/走り/壁のぼりの移動速度を切り替える処理を担当するクラスが実装する
    /// </summary>
    public interface ISpeedSwitchable
    {
        public void Walk();

        public void DisposeWalkSubscription();
    }
}

