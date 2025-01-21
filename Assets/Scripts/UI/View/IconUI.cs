using UnityEngine;
using DG.Tweening;
using UI.Base;
using UI.Interface;

namespace UI.View
{
    /// <summary>
    /// アイコンの拡大縮小を行う
    /// </summary>
    public class IconUI : UIElementBase, IIconUI
    {
        [SerializeField] private Transform _iconTransform;
        private Vector3 _originalScale;

        protected override void Awake()
        {
            base.Awake();
            _originalScale = _iconTransform.localScale;
        }

        /// <summary>
        /// 選択された時に呼び出す
        /// アイコンを拡大する
        /// </summary>
        public void Select()
        {
            _iconTransform.DOScale(_originalScale * 1.2f, 0.3f).SetEase(Ease.OutBack);
        }

        /// <summary>
        /// 選択解除された時に呼び出す
        /// アイコンを縮小する
        /// </summary>
        public void Deselect()
        {
            _iconTransform.DOScale(_originalScale, 0.3f).SetEase(Ease.OutBack);
        }
    }
}