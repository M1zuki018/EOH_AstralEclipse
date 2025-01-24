namespace UI.Interface
{
    /// <summary>
    /// ゲージのインターフェース
    /// </summary>
    public interface IGaugeUI : IUIElement
    {
        void SetValue(float endValue, float duration);
    }
}