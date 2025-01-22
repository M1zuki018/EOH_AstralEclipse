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
        /// スライダーの初期化を行う 
        /// </summary>
        public void InitializeValue(int maxValue, int defaultValue)
        {
            _slider.maxValue = maxValue;
            _slider.value = defaultValue;
        }
        
        /// <summary>
        /// スライダーを滑らかに更新する
        /// </summary>
        public void SetValue(int value)
        {
            _slider.DOValue(value, _updateDuration).SetEase(Ease.OutQuad);
        }
    }

}
