using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayerSystem.State;
using UI.Interface;
using UniRx;
using UnityEngine;

/// <summary>
/// 歩行中/走行中の切り替えをわかりやすくするUI
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class IsWalkingTextUI : ViewBase
{
    [SerializeField] private PlayerBrain _playerBrain;
    private CanvasGroup _canvasGroup;
    private IDisposable _subscription;
    
    public override UniTask OnStart()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        var blackboard = _playerBrain.BB;
        _subscription = blackboard.IsWalking.Subscribe(_ => Show().Forget()); // 移動速度が変更されたら表示
        _canvasGroup.alpha = 0;
        
        return base.OnStart();
    }
    
    private async UniTask Show()
    {
        _canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutQuad);
        _canvasGroup.transform.localScale = Vector3.one;
        
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        
        Hide();
    }

    public void Hide()
    {
        _canvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutQuad);
        _canvasGroup.transform.DOScale(0.95f, 2f).SetEase(Ease.OutQuad);
    }

    private void OnDestroy()
    {
        _subscription?.Dispose();
    }
}
