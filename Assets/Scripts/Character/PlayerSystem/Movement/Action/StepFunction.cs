using UnityEngine;
using UniRx;
using System;
using PlayerSystem.ActionFunction;
using PlayerSystem.Animation;
using PlayerSystem.State;

/// <summary>
/// ステップ機能を提供する
/// </summary>
public class StepFunction : ISteppable
{
    private PlayerAnimationController _animController;
    private PlayerBlackBoard _bb;
    
    private CompositeDisposable _disposable = new CompositeDisposable(); //Subscribeを登録しておく

    public StepFunction(PlayerAnimationController animController, PlayerBlackBoard bb)
    {
        _animController = animController;
        _bb = bb;
        
        UIManager.Instance?.HideStepUI(); //UIを隠す
        _bb.CurrentSteps = _bb.Data.MaxSteps; // ステップ数の初期化
    }

    /// <summary>
    /// ステップ機能
    /// </summary>
    public void Step()
    {
        if (_bb.CurrentSteps == _bb.Data.MaxSteps)
        {
            // ステップ数が最大値の状態から変更される場合、時間経過で回復する処理の購読を開始する
            StartStepRecovery();
            UIManager.Instance?.UpdateStepGauge(1,_bb.Data.RecoveryTime);
        }
        
        //ステップ回数を減らすのと、UIを更新する
        _bb.CurrentSteps--;
        UIManager.Instance?.UpdateStepCount(_bb.CurrentSteps);
        
        //ステップアニメーションをトリガーする
        _animController.Movement.PlayStepAnimation();
    }
    
    /// <summary>
    /// ステップ回数を回復する
    /// </summary>
    private void StartStepRecovery()
    {
        UIManager.Instance.ShowStepUI(); //UIを見せる
        
        // 一定間隔でステップを回復する
        Observable.Interval(TimeSpan.FromSeconds(_bb.Data.RecoveryTime))
            .Where(_ => _bb.CurrentSteps < _bb.Data.MaxSteps)  // ステップが最大値以下の場合のみ回復
            .Subscribe(_ =>
            {
                _bb.CurrentSteps++;
                UIManager.Instance?.UpdateStepCount(_bb.CurrentSteps);
                
                if (_bb.CurrentSteps >= _bb.Data.MaxSteps)
                {
                    StopStepRecovery(); //もし最大回数になっていたら購読を解除する
                    return;
                }
                
                UIManager.Instance?.UpdateStepGauge(1,_bb.Data.RecoveryTime);
            })
            .AddTo(_disposable); // GameObjectが破棄されるときに購読を解除
    }

    /// <summary>
    /// ステップ回数回復を止める
    /// </summary>
    private void StopStepRecovery()
    {
        _disposable.Clear(); //購読を解除する
        UIManager.Instance?.HideStepUI(); //UIを隠す
    }
}