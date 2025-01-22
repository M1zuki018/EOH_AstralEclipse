namespace UI.Interface
{
    /// <summary>
    /// スライダーのインターフェース
    /// </summary>
    public interface ISliderUI : IUIElement
    {
        void SetValue(int value);
        void InitializeValue(int maxValue, int defaultValue);
    }
}

