using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

/// <summary>
/// DOTweenのTweenをUniTaskに変換する
/// </summary>
public static class DOTweenAsyncExtensions
{
    public static UniTask ToUniTask(this Tween tween, CancellationToken cancellationToken = default)
    {
        // UniTaskCompletionSource を作成して、Tween 完了を待つ非同期処理を実装
        var taskCompletionSource = new UniTaskCompletionSource();

        // 完了時に UniTaskCompletionSource を設定
        tween.OnKill(() => taskCompletionSource.TrySetResult());  // Tween がキルされると完了
        tween.OnComplete(() => taskCompletionSource.TrySetResult());  // Tween が完了したときにも完了

        // キャンセル用に設定 (キャンセルが要求された場合、完了させる)
        cancellationToken.Register(() => taskCompletionSource.TrySetCanceled());

        // UniTask として返す
        return taskCompletionSource.Task;
    }
}