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
    private CompositeDisposable _disposable = new CompositeDisposable(); //Subscribeを登録しておく

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>(); //Animator,State取得用
        
        OnStep += HandleStep;
        
        UIManager.Instance.HideStepUI(); //UIを隠す
        _currentSteps = _maxSteps; // ステップ数の初期化

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
        else
        {
            Debug.Log("ステップカウントが足りません！");
        }
    }
    
    /// <summary>
    /// ステップ機能
    /// </summary>
    public void HandleStep()
    {
        if (_currentSteps == _maxSteps)
        {
            StartStepRecovery();
            UIManager.Instance.UpdateStepGauge(1,_recoveryTime);
        }
        
        //ステップ回数を減らすのと、UIを更新する
        _currentSteps--;
        UIManager.Instance.UpdateStepCount(_currentSteps);

        Vector3 velocity = _playerMovement.PlayerState.MoveDirection;
        float moveSpeed = _playerMovement.PlayerState.MoveSpeed;
        
        if (velocity.magnitude < 0.1f) //入力がなかったら正面方向にステップ
        {
            _playerMovement._animator.SetFloat("XVelocity", 1);
            _playerMovement._animator.SetFloat("ZVelocity", 0);            
        }
        else //入力があれば、それに応じた方向にステップできるようにAnimatorに値を渡す
        {
            _playerMovement._animator.SetFloat("XVelocity", velocity.x * moveSpeed);
            _playerMovement._animator.SetFloat("ZVelocity", velocity.z * moveSpeed);
        }
        
        //ステップアニメーションをトリガーする
        _playerMovement._animator.SetTrigger("Step");
        
        //AudioManager.Instance.PlaySE(7);
    }
    
    /// <summary>
    /// ステップ回数を回復する
    /// </summary>
    private void StartStepRecovery()
    {
        UIManager.Instance.ShowStepUI(); //UIを見せる
        
        // 一定間隔でステップを回復する
        Observable.Interval(TimeSpan.FromSeconds(_recoveryTime))
            .Where(_ => _currentSteps < _maxSteps)  // ステップが最大値以下の場合のみ回復
            .Subscribe(_ =>
            {
                _currentSteps++;
                UIManager.Instance.UpdateStepCount(_currentSteps);
                
                if (_currentSteps >= _maxSteps)
                {
                    StopStepRecovery(); //もし最大回数になっていたら購読を解除する
                    return;
                }
                
                UIManager.Instance.UpdateStepGauge(1,_recoveryTime);
            })
            .AddTo(_disposable); // GameObjectが破棄されるときに購読を解除
    }

    /// <summary>
    /// ステップ回数回復を止める
    /// </summary>
    private void StopStepRecovery()
    {
        _disposable.Clear(); //購読を解除する
        UIManager.Instance.HideStepUI(); //UIを隠す
    }
}