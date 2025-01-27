using UI.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View
{
    /// <summary>
    /// ロックオンのUIを管理する
    /// </summary>
    public class LockOnUI : MonoBehaviour, IIconUI
    {
        [SerializeField, HighlightIfNull] private Image _icon;
        [SerializeField, HighlightIfNull] private CanvasGroup _canvasGroup;
        [SerializeField, Comment("頭上のオフセット")] private Vector3 offset = new Vector3(0, 2.5f, 0);
        
        private Camera _mainCamera;
        private Transform _target; //アイコンを表示するエネミーのTransform
        private RectTransform _rectTransform;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            //非表示の時とtargetがいない時は、以降の処理を行わない
            if (!_icon.gameObject.activeSelf || _target == null) return; 
            
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
        private void SetTarget(Transform targetTransform)
        {
            _target = targetTransform;
        }

        /// <summary>
        /// オブジェクトを表示する
        /// </summary>
        public void IsActive(Transform targetTransform)
        {
            _icon.gameObject.SetActive(true);
            SetTarget(targetTransform);
        }
        
        /// <summary>
        /// オブジェクトを非表示にする
        /// </summary>
        public void IsNotActive()
        {
            _icon.gameObject.SetActive(false);
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

        //以下の処理はない
        public void Select() { }
        public void Deselect() { }
    }

}
