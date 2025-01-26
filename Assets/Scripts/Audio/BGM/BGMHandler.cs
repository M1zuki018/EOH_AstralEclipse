using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGM再生を管理する
/// </summary>
public class BGMHandler : MonoBehaviour
{
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioDataSO _bgmData;
    [SerializeField] private Door _door;

    private void Start()
    {
        _door.OnDoorOpened += HandleDoorOpened; //ボス戦突入時のイベント
    }

    private void OnDestroy()
    {
        _door.OnDoorOpened -= HandleDoorOpened;
    }

    /// <summary>
    /// ボス戦突入時のBGM変更
    /// </summary>
    private void HandleDoorOpened()
    {
 //       _bgmSource.clip = _bgmData.;
    }
}
