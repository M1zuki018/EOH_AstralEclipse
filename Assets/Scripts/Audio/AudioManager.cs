using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Audio全体を管理するクラス
/// </summary>
public class AudioManager : ViewBase
{
    public static AudioManager Instance;
    
    [Header("再生するBGMの設定")]
    [SerializeField] private int _defaultBGMIndex = 0;
    private AudioType _myType = AudioType.BGM;
    
    [Header("Sound系の設定")]
    [SerializeField] private List<AudioSource> _audioSources = new List<AudioSource>();
    [SerializeField] private List<AudioDataSO> _audioDatas = new List<AudioDataSO>();
    [SerializeField] private AudioMixer _bgmMixer;

    public override UniTask OnAwake()
    {
        Instance = this;
        return base.OnAwake();
    }

    public override UniTask OnBind()
    {
        GameManager.Instance.OnMovie += GameStart;
        return base.OnBind();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnMovie -= GameStart;
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
    /// ゲーム開始時にBGMを初期化する処理
    /// </summary>
    private void GameStart()
    {
        ClipChange(_myType, _defaultBGMIndex); //BGMを初期化
        FadeIn(_myType);
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
    /// ボイスを再生する
    /// </summary>
    public void PlayVoice(int clipIndex)
    {
        ClipData clip = GetClipData(_audioDatas[2], clipIndex);
        _audioSources[2].volume = clip.Volume;
        _audioSources[2].PlayOneShot(clip.Clip);
    }

    /// <summary>
    /// フェードイン機能
    /// </summary>
    public void FadeIn(AudioType audioType, float duration = 0.5f)
    {
        int index = GetAudioIndex(audioType);
        float volume = _audioSources[index].volume;
        _audioSources[index].DOFade(volume, duration);
    }
    
    /// <summary>
    /// フェードアウト機能
    /// </summary>
    public void FadeOut(AudioType audioType, float duration = 0.5f)
    {
        _audioSources[GetAudioIndex(audioType)].DOFade(0, duration);
    }

    /// <summary>
    /// AudioTypeに対応したIndex番号を返します
    /// </summary>
    private int GetAudioIndex(AudioType audioType) => audioType switch
    {
        AudioType.BGM => 0,
        AudioType.SE => 1,
        AudioType.Voice => 2,
        AudioType.Environment => 3,
        _ => -1
    };

    /// <summary>Clipデータの構造体を返します</summary>
    private ClipData GetClipData(AudioDataSO audioData, int index) => audioData.Clips[index];
}
