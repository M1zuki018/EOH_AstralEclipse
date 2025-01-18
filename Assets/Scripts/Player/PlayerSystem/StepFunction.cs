using UnityEngine;
using UniRx;
using System;

/// <summary>
/// ステップ機能を提供する
/// </summary>
public class StepFunction : MonoBehaviour
{
    [SerializeField] private int _maxSteps = 10; // ステップの最大数
    [SerializeField] private float _recoveryTime = 5f; // 回復間隔（秒）
    private int _currentSteps; // 現在のステップ数

    private void Start()
    {
        _currentSteps = _maxSteps; // ステップの初期化

        // 一定間隔でステップを回復する
        Observable.Interval(TimeSpan.FromSeconds(_recoveryTime))
            .Where(_ => _currentSteps < _maxSteps)  // ステップが最大値以下の場合のみ回復
            .Subscribe(_ =>
            {
                _currentSteps++;
                Debug.Log($"Steps recovered: {_currentSteps}/{_maxSteps}");
            })
            .AddTo(this); // GameObjectが破棄されるときに購読を解除
    }

    /// <summary>
    /// ステップを消費する
    /// </summary>
    public bool TryUseStep()
    {
        if (_currentSteps > 0)
        {
            _currentSteps--;
            Debug.Log($"Step used: {_currentSteps}/{_maxSteps}");
            return true;
        }
        Debug.Log("No steps available!");
        return false;
    }
}