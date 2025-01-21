using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UI.Base;
using UI.Interface;

namespace UI.View
{
    /// <summary>
    /// ゲージUIのFillを操作する
    /// </summary>
    public class GaugeUI : UIElementBase, IGaugeUI
    {
        [SerializeField] private Image _gaugeImage;

        /// <summary>
        /// ゲージUIのFillを操作する
        /// </summary>
        public void SetValue(float normalizedValue)
        {
            _gaugeImage.DOFillAmount(normalizedValue, 0.5f).SetEase(Ease.OutQuad);
        }
    }
}