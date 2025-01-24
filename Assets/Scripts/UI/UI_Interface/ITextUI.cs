using UI.Base;

namespace UI.Interface
{
    /// <summary>
    /// 文字のUIのインターフェース
    /// </summary>
    public interface ITextUI : IUIElement
    {
        void SetText(string text);
    }

}
