namespace UI.Interface
{
    /// <summary>
    /// スライダーのインターフェース
    /// </summary>
    public interface ISliderUI : IUIElement
    {
        void SetValue(float normalizedValue);
    }
}

