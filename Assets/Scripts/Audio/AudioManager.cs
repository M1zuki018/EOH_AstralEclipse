using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio全体を管理するクラス
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioSource> _audioSources = new List<AudioSource>();
    [SerializeField] private List<AudioDataSO> _audioDatas = new List<AudioDataSO>();

    /// <summary>
    /// AudioSourceのクリップを変更する
    /// </summary>
    public void ClipChange(AudioType audioType, int clipIndex)
    {
        int index = GetAudioIndex(audioType);
        AudioSource source = GetAudioSource(index);
        ClipData clip = GetClipData(_audioDatas[index], clipIndex);
        
        //取得したAudioSourceにクリップの情報を設定する
        source.clip = clip.Clip;
        source.volume = clip.Volume;
        source.loop = clip.Loop;
        
        source.Play();
    }

    public void PlayOnShotClip(AudioType audioType, int clipIndex)
    {
        //TODO:実装を書く
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

    /// <summary>AudioSourceを返します</summary>
    public AudioSource GetAudioSource(int index) => _audioSources[index];
    
    /// <summary>Clipデータの構造体を返します</summary>
    public ClipData GetClipData(AudioDataSO audioData, int index) => audioData.Clips[index];
}
