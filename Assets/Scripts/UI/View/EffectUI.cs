using UnityEngine;
using DG.Tweening;
using UI.Base;
using UI.Interface;

namespace UI.View
{
    /// <summary>
    /// UIエフェクトを管理する
    /// </summary>
    public class EffectUI : UIElementBase, IEffectUI
    {
        [SerializeField] private CanvasGroup _effectCanvas;

        /// <summary>
        /// 点滅させる
        /// </summary>
        public void PlayEffect()
        {
            _effectCanvas.DOFade(0.5f, 0.2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }
    }
}