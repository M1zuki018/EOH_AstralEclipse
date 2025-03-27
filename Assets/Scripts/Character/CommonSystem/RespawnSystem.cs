using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// ステージ外に落下した時の処理
/// </summary>
public class RespawnSystem : ViewBase
{
    [Header("設定")] 
    [SerializeField, Comment("落下したとみなすY座標")] private float _fallHeight = -3f;
    [SerializeField, HighlightIfNull] private Transform _player;
    
    public event Action OnRespawn; //落下時のイベント

    private void Start()
    {
        MonitorFall();
    }

    /// <summary>
    /// プレイヤーの座標がFallHeightで設定したY座標を下回ったらリスポーンイベントを発火する
    /// </summary>
    public void MonitorFall()
    {
        Observable
            .EveryUpdate()
            .Where(_ => _player.position.y < _fallHeight)
            .Take(1)
            .Subscribe(_ =>
            {
                OnRespawn?.Invoke();
            })
            .AddTo(this);
    }
}
