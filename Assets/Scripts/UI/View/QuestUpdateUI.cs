using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// クエストが更新されたときのアニメーション
/// </summary>
public class QuestUpdateUI : MonoBehaviour
{
    [SerializeField] private RectTransform _target; // UI オブジェクト (例: パネル)
    [SerializeField] private Vector2 _startPosition; // 開始位置
    [SerializeField] private Vector2 _endPosition; // 終了位置
    [SerializeField] private CanvasGroup _canvasGroup;

    public async UniTaskVoid ShowAsync()
    {
        if (_target == null) return;

        // 初期位置の設定
        _target.anchoredPosition = _startPosition;
        _canvasGroup.alpha = 0f;

        // 横にスライドしながら透明度を上げる
        await UniTask.WhenAll(
            _target.DOAnchorPos(new Vector2(_startPosition.x + 50, _startPosition.y), 0.1f).SetEase(Ease.InCirc).ToUniTask(),
            _canvasGroup.DOFade(0.7f, 0.1f).SetEase(Ease.InCirc).ToUniTask()
        );

        // 少し待機
        await UniTask.Delay(100);

        // 透明度を0にしつつスライド
        await UniTask.WhenAll(
            _target.DOAnchorPos(_endPosition, 0.5f).SetEase(Ease.OutQuad).ToUniTask(),
            _canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.OutQuad).ToUniTask()
        );
    }

    // ボタンやイベントから呼び出せるようにする
    public void Show()
    {
        ShowAsync().Forget();
    }
}
