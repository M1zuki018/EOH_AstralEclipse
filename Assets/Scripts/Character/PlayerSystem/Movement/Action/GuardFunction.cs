using System;
using PlayerSystem.ActionFunction;
using PlayerSystem.State;
using UniRx;
using UnityEngine;

/// <summary>
/// ガード機能を提供する
/// </summary>
public class GuardFunction : IGuardable
{
    //ガードブレイク＝willの値が削り切られてしまったらブレイク状態

    private PlayerBlackBoard _bb;
    private CompositeDisposable _disposables = new CompositeDisposable();
    
    private IDisposable _guardDisposable;

    public GuardFunction(PlayerBlackBoard bb)
    {
        _bb = bb;
    }

    /// <summary>
    /// ガードし始めたときの処理
    /// </summary>
    public void GuardStart()
    {
        _bb.AnimController.Movement.PlayGuardAnimation();
        _bb.IsGuarding = true;

        StopDispose();
        
        // ガード中だけ、1秒に1ずつwillを減らしていく処理
        _guardDisposable = Observable
            .Interval(TimeSpan.FromSeconds(1))
            .TakeWhile(_ => _bb.IsGuarding)
            .Subscribe(_ =>
            {
                _bb.CurrentWill = Math.Max(0, _bb.CurrentWill - 1);
                UIManager.Instance.UpdatePlayerWill(_bb.CurrentWill);
            })
            .AddTo(_disposables);
    }

    /// <summary>
    /// ガード中の処理
    /// </summary>
    public void Guard()
    {
        if (_bb.CurrentWill <= 0)
        {
            _bb.IsGuardBreak = true; // ガードブレイク状態にする
            GuardEnd();
        }
    }

    /// <summary>
    /// ガードをやめるときの処理
    /// </summary>
    public void GuardEnd()
    {
        StopDispose();
        _bb.AnimController.Movement.StopGuardAnimation();
        _bb.IsGuarding = false;
    }

    private void StopDispose()
    {
        _guardDisposable?.Dispose();
        _guardDisposable = null;
    }
}
