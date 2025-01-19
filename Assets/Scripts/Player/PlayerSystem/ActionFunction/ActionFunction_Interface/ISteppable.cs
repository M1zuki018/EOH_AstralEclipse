namespace PlayerSystem.ActionFunction
{
    public interface ISteppable
    {
        /// <summary>ステップを消費する</summary>
        public bool TryUseStep();

        /// <summary>現在のステップ数を取得する</summary>
        public int CurrentSteps { get; }
        
        /// <summary>最大ステップ数を取得する</summary>
        public int MaxSteps { get; }
        
        /// <summary>ステップ機能</summary>
        public void Step();
    }
}