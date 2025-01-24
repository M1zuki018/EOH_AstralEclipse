using UnityEngine;
using UniRx;
using System;
using PlayerSystem.ActionFunction;
using PlayerSystem.Movement;

/// <summary>
/// ステップ機能を提供する
/// </summary>
public class StepFunction : MonoBehaviour, ISteppable
{
    [SerializeField, Comment("ステップの最大数")] private int _maxSteps = 10;
    [SerializeField, Comment("回復間隔（秒）")] private float _recoveryTime = 5f;
    private int _currentSteps; // 現在のステップ数
    private PlayerMovement _playerMovement;

    public int CurrentSteps => _currentSteps; //現在のステップ数（読み取り専用）
    public int MaxSteps => _maxSteps; //最大ステップ数（読み取り専用）
    public event Action OnStep;

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        
        OnStep += HandleStep;
        
        _currentSteps = _maxSteps; // ステップ数の初期化

        // 一定間隔でステップを回復する
        Observable.Interval(TimeSpan.FromSeconds(_recoveryTime))
            .Where(_ => _currentSteps < _maxSteps)  // ステップが最大値以下の場合のみ回復
            .Subscribe(_ =>
            {
                _currentSteps++;
                Debug.Log($"ステップ回数が回復しました: {_currentSteps}/{_maxSteps}");
            })
            .AddTo(this); // GameObjectが破棄されるときに購読を解除
    }

    private void OnDestroy()
    {
        OnStep -= HandleStep; //イベント解除
    }

    /// <summary>
    /// ステップを消費する
    /// </summary>
    public void TryUseStep()
    {
        if (_currentSteps > 0)
        {
            OnStep?.Invoke();
        }
        Debug.Log("ステップカウントが足りません！");
    }
    
    /// <summary>
    /// ステップ機能
    /// </summary>
    public void HandleStep()
    {
        _currentSteps--;
        _playerMovement._animator.SetTrigger("Step");
        Debug.Log($"Step used: {_currentSteps}/{_maxSteps}");
    }
}