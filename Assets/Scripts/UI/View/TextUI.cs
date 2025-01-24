using UI.Base;
using UI.Interface;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// テキストを書き換える
/// </summary>
public class TextUI : UIElementBase, ITextUI
{
    [SerializeField] private Text _text;
    
    /// <summary>
    /// テキストを更新する
    /// </summary>
    public void SetText(string text)
    {
        _text.text = text;
    }
}
