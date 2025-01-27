
using UI.Interface;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ダメージの数字を表示します
/// </summary>
public class DamageAmountUI : MonoBehaviour, ITextUI
{
    [SerializeField, HighlightIfNull] private Text _text;
    [SerializeField, HighlightIfNull] private CanvasGroup _canvasGroup;
    [SerializeField, Comment("オフセット")] private float _offset;
    [SerializeField, Comment("数字の散らばる範囲")] private Vector2 _dispersion;
    
    private Camera _mainCamera;
    private Transform _target; //ダメージを表示するキャラクターのTransform
    private RectTransform _rectTransform;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// ダメージ量を表示する
    /// </summary>
    public void Show(int damage)
    {
        SetText(damage.ToString());
    }
    
    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// ダメージ量を書き換える
    /// </summary>
    public void SetText(string text)
    {
        _text.text = text;
    }
    
    //インターフェースのメソッドは使用しない
    public void Show() { }
    
}
