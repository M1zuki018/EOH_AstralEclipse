using UnityEngine;
using DG.Tweening;
using UI.Base;
using UI.Interface;
using UnityEngine.UI;

namespace UI.View
{
    /// <summary>
    /// 数値UIの値を書き換える
    /// </summary>
    public class NumberUI : UIElementBase, INumberUI
    {
        [SerializeField] private Text _numberText;

        /// <summary>
        /// 数字を少しずつ変更する
        /// </summary>
        /// <param name="value">書き換え完了後の数字</param>
        public void SetNumber(int value)
        {
            int startValue = int.Parse(_numberText.text);
            DOTween.To(() => startValue, x => _numberText.text = x.ToString(), value, 0.5f)
                .SetEase(Ease.Linear);
        }
    }
}