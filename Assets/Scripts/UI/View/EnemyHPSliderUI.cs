using UI.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View
{
    /// <summary>
    /// エネミーのHPスライダーUIを管理する
    /// </summary>
    public class EnemyHPSliderUI : MonoBehaviour, ISliderUI
    { 
        [SerializeField, HighlightIfNull] private Slider _healthSlider;
        [SerializeField, HighlightIfNull] private CanvasGroup _canvasGroup;
        [SerializeField, Comment("頭上のオフセット")] private Vector3 offset = new Vector3(0, 2.0f, 0);
        
        private Camera _mainCamera;
        private Transform _target; // エネミーの Transform
        private RectTransform _rectTransform;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            // ワールド座標をスクリーン座標に変換
            Vector3 worldPosition = _target.position + offset;
            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);

            // 敵がカメラの前にいるかチェック（カメラの後ろなら非表示）
            if (screenPosition.z > 0)
            {
                _rectTransform.position = screenPosition;
                Show();
            }
            else
            {
                Hide();
            }
        }

        /// <summary>
        /// ターゲットとなるエネミーのトランスフォームをセットする
        /// </summary>
        public void SetTarget(Transform targetTransform)
        {
            _target = targetTransform;
        }
        
        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// スライダーを更新する実装
        /// </summary>
        public void SetValue(int value)
        {
            _healthSlider.value = value;
            Show();
        }

        /// <summary>
        /// スライダーを初期化する
        /// </summary>
        public void InitializeValue(int maxValue, int defaultValue)
        {
            _healthSlider.maxValue = maxValue;
            _healthSlider.value = maxValue;
        }
    }
}


