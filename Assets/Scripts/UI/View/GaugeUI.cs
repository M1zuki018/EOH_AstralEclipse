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
        public void SetValue(float endValue, float duration)
        {
            _gaugeImage.DOFillAmount(endValue, duration).SetEase(Ease.Linear);
        }
        
        /// <summary>
        /// ゲージUIのFillをリセットした後操作する
        /// </summary>
        public void ResetAndSetValue(float endValue, float duration)
        {
            _gaugeImage.fillAmount = 0;
            _gaugeImage.DOFillAmount(endValue, duration).SetEase(Ease.Linear);
        }
    }
}