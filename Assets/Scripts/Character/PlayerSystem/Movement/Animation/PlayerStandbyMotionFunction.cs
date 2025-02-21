using System;
using System.Collections.Generic;
using PlayerSystem.Animation;
using PlayerSystem.State;
using UniRx;
using UnityEngine.InputSystem;

/// <summary>
/// キャラクターの待機モーション再生を管理するクラス
/// </summary>
public class PlayerStandbyMotionFunction : IDisposable
{
    //Idleモーション再生のための変数
    private float _idleThreshold = 7f; //無操作とみなす秒数
    private Subject<Unit> _inputDetected = new Subject<Unit>();

    private PlayerBlackBoard _bb;
    private PlayerAnimationController _animController;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    public PlayerStandbyMotionFunction(PlayerBlackBoard bb, PlayerAnimationController animController)
    {
        _bb = bb;
        _animController = animController;
        Initialize();
    }
    
    private void Initialize()
    {
        SubscribeToInputEvents(); //入力イベントを購読

        _inputDetected
            .Throttle(TimeSpan.FromSeconds(_idleThreshold)) //最後の入力から指定した間入力がなかったら以下の処理を行う
            .Subscribe(_ => _animController.Common.PlayRandomIdleMotion())
            .AddTo(_disposables);
    }
    
    /// <summary>
    /// プレイヤーの入力を監視する
    /// </summary>
    private void SubscribeToInputEvents()
    {
        // 全てのアクションからObservableを作成
        var moveActionStreams = new List<IObservable<InputAction.CallbackContext>>();
        foreach (var action in _bb.MoveActions)
        {
            moveActionStreams.Add(action.action.PerformedAsObservable()); //InputActionをObservableに変換する
        }

        // 全てのアクションをマージして監視
        moveActionStreams
            .Merge() // Observableを1つに統合
            .Subscribe(_ =>
            {
                _inputDetected.OnNext(Unit.Default); // 入力があった時、通知を行う
                _animController.Common.StopIdleMotion(); //Idleモーションを中断
            })
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables?.Dispose();
    }
}
