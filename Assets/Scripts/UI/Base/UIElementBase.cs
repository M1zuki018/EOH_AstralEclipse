using DG.Tweening;
using UI.Interface;
using UnityEngine;

namespace UI.Base
{
    /// <summary>
    /// 全UIの基底クラス
    /// </summary>
    public class UIElementBase : MonoBehaviour, IUIElement
    {
        protected CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// UIを出現させる
        /// </summary>
        public virtual void Show()
        {
            _canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutQuad);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// UIを隠す
        /// </summary>
        public virtual void Hide()
        {
            _canvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutQuad);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        
        /// <summary>
        /// UIを左からスライドさせながら出現させる
        /// </summary>
        public virtual void ShowAndSlide()
        {
            _canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutQuad);
            _canvasGroup.transform.DOMoveX(_canvasGroup.transform.position .x + 50, 0.3f).SetEase(Ease.OutQuad);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// UIを左にスライドさせながら隠す
        /// </summary>
        public virtual void HideAndSlide()
        {
            _canvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutQuad);
            _canvasGroup.transform.DOMoveX(_canvasGroup.transform.position .x - 50, 0.3f).SetEase(Ease.OutQuad);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}
