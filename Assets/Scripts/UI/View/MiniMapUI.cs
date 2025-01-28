using UI.Interface;
using UnityEngine;

public class MiniMapUI : MonoBehaviour, IIconUI
{
    [SerializeField] private CanvasGroup _canvasGroup;
    
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

    public void Select() { }

    public void Deselect() {}
}
