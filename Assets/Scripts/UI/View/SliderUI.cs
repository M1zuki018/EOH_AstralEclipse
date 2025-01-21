using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UI.Base;
using UI.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View
{
    /// <summary>
    /// スライダーを滑らかに更新する
    /// </summary>
    public class SliderUI : UIElementBase, ISliderUI
    {
        [SerializeField, HighlightIfNull] private Slider _slider;
        [SerializeField] private float _updateDuration = 0.5f; //アニメーションの時間

        protected override void Awake()
        {
            base.Awake();
            if (_slider == null)
            {
                //もしアサインされていなかったらコンポーネントを取得する
                _slider = GetComponent<Slider>();
            }
        }
        
        /// <summary>
        /// スライダーを滑らかに更新する
        /// </summary>
        /// <param name="normalizedValue">0-1の間に正規化した状態の値</param>
        public void SetValue(float normalizedValue)
        {
            _slider.DOValue(normalizedValue, _updateDuration).SetEase(Ease.OutQuad);
        }
    }

}
