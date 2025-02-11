using DG.Tweening;
using TMPro;
using UI.Interface;
using UnityEngine;

/// <summary>
/// プレイ開始時の「GameStart」赤文字のアニメーションクラス
/// </summary>
public class GameStartUI : MonoBehaviour, IUIElement
{
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _text;

    public void Show()
    {
        _canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutQuad).SetUpdate(true);
        _canvasGroup.transform.localScale = Vector3.one;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        _canvasGroup.DOFade(0, 1f).SetEase(Ease.OutQuad).SetUpdate(true);
        _canvasGroup.transform.DOScale(0.95f, 2f).SetEase(Ease.OutQuad).SetUpdate(true);
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}
