/// <summary>
/// BGM、SEの操作をする
/// </summary>
public class PlayerAudioController
{
    /// <summary>
    /// ダメージを喰らったときのSEを再生
    /// </summary>
    public void HitSE() => AudioManager.Instance?.PlaySE(14);

    /// <summary>
    /// BGMとSEをフェードアウトする
    /// </summary>
    public void FadeOut()
    {
        AudioManager.Instance.FadeOut(AudioType.BGM);
        AudioManager.Instance.FadeOut(AudioType.SE);
    }
}
