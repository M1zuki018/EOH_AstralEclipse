using System;
using UnityEngine;

/// <summary>
/// BGM再生を管理する
/// </summary>
public class BGMHandler : MonoBehaviour
{
    [SerializeField] private int _defaultBGMIndex = 0;
    private AudioType _myType = AudioType.BGM;

    private void Start()
    {
        GameManager.Instance.OnPlay += GameStart;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPlay -= GameStart;
    }

    private void GameStart()
    {
        AudioManager.Instance.ClipChange(_myType, _defaultBGMIndex); //BGMを初期化
        AudioManager.Instance.FadeIn(_myType);
    }
}
