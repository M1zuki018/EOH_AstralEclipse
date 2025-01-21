using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// DOTween共通処理をまとめたクラス
/// ※おためし作成中
/// </summary>
public static class TweenHelper
{
    /// <summary>
    /// フェードイン・アウト
    /// </summary>
    public static Tween Fade(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        return canvasGroup.DOFade(targetAlpha, duration);
    }

    /// <summary>
    /// 移動アニメーション
    /// </summary>
    public static Tween Move(Transform target, Vector3 endValue, float duration, Ease ease = Ease.Linear)
    {
        return target.DOMove(endValue, duration).SetEase(ease);
    }

    /// <summary>
    /// スケールアニメーション
    /// </summary>
    public static Tween Scale(Transform target, Vector3 endScale, float duration, Ease ease = Ease.OutBounce)
    {
        return target.DOScale(endScale, duration).SetEase(ease);
    }
    
    /// <summary>
    /// スライダーアニメーション
    /// </summary>
    public static Tween AnimateSlider(Slider slider, float targetValue, float duration)
    {
        return slider.DOValue(targetValue, duration).SetEase(Ease.OutQuad);
    }
}
