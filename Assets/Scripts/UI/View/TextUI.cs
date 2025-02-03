using TMPro;
using UI.Base;
using UI.Interface;
using UnityEngine;

namespace UI.View
{
    /// <summary>
    /// テキストを書き換える
    /// </summary>
    public class TextUI : UIElementBase, ITextUI
    {
        [SerializeField] private TMP_Text _text;
    
        /// <summary>
        /// テキストを更新する
        /// </summary>
        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}

