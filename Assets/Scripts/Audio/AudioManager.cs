using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Audio全体を管理するクラス
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [SerializeField] private List<AudioSource> _audioSources = new List<AudioSource>();
    [SerializeField] private List<AudioDataSO> _audioDatas = new List<AudioDataSO>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //インスタンスを生成
        }
        else
        {
            Destroy(gameObject); //既にあったら破棄する
        }
    }
    
    /// <summary>
    /// AudioSourceのクリップを変更する
    /// </summary>
    public void ClipChange(AudioType audioType, int clipIndex)
    {
        int index = GetAudioIndex(audioType);
        AudioSource source = _audioSources[index];
        ClipData clip = GetClipData(_audioDatas[index], clipIndex);
        
        //取得したAudioSourceにクリップの情報を設定する
        source.clip = clip.Clip;
        source.volume = clip.Volume;
        source.loop = clip.Loop;
        
        source.Play();
    }

    /// <summary>
    /// SEを再生する
    /// </summary>
    public void PlaySE(int clipIndex)
    {
        //音源を取得したら、volumeを調整してから再生する
        ClipData clip = GetClipData(_audioDatas[1], clipIndex);
        _audioSources[1].volume = clip.Volume;
        _audioSources[1].PlayOneShot(clip.Clip);
    }

    /// <summary>
    /// 一定秒数待ってからSEを再生する
    /// </summary>
    public async void PlaySEDelay(int clipIndex, int delay)
    {
        await Task.Delay(delay);
        ClipData clip = GetClipData(_audioDatas[1], clipIndex);
        _audioSources[1].volume = clip.Volume;
        _audioSources[1].PlayOneShot(clip.Clip);
    }

    /// <summary>
    /// フェードイン機能
    /// </summary>
    public void FadeIn(AudioType audioType)
    {
        int index = GetAudioIndex(audioType);
        float volume = _audioSources[index].volume;
        _audioSources[index].DOFade(volume, 0.5f);
    }
    
    /// <summary>
    /// フェードアウト機能
    /// </summary>
    public void FadeOut(AudioType audioType)
    {
        _audioSources[GetAudioIndex(audioType)].DOFade(0, 0.5f);
    }
    
    /// <summary>
    /// AudioTypeに対応したIndex番号を返します
    /// </summary>
    private int GetAudioIndex(AudioType audioType)
    {
        //enumに応じて、返すAudioSourceのindex番号を取得
        int index = audioType switch
        {
            AudioType.BGM => 0,
            AudioType.SE => 1,
            AudioType.Voice => 2,
            AudioType.Environment => 3,
            _ => 0
        };

        return index;
    }

    /// <summary>Clipデータの構造体を返します</summary>
    private ClipData GetClipData(AudioDataSO audioData, int index) => audioData.Clips[index];
}
