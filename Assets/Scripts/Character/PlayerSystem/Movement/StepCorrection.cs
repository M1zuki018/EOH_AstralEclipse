using UniRx;
using UnityEngine;
using System;

/// <summary>
/// ステップモーションに補正をかけるクラス
/// </summary>
public class StepCorrection : StateMachineBehaviour
{
    [SerializeField, Comment("補正をはじめる時間")] private float _correctionStartTime = 0.2f;
    [SerializeField, Comment("補正を終える時間")] private float _correctionEndTime = 0.6f;
    [SerializeField, Comment("移動スピード")] private float _correctionSpeed = 3.0f;
    
    private Vector3 _moveDirection;
    private CharacterController _cc;
    private PlayerMovement _playerMovement;
    private CompositeDisposable _disposables = new CompositeDisposable();
    
    private void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_cc == null)_cc = animator.GetComponent<CharacterController>();
        if(_playerMovement == null)_playerMovement = animator.GetComponent<PlayerMovement>();

        _playerMovement.PlayerState.IsSteping = true; //ステップ状態にする

        Vector3 direction = _playerMovement.PlayerState.MoveDirection.normalized;

        
        if (direction.magnitude > 0.05f)
        {
            //入力があったらプレイヤーの入力を保存しておく
            _moveDirection = _playerMovement.PlayerState.MoveDirection.normalized;
        }
        else
        {
            //入力がなかったらキャラクターの正面方向を保存しておく
            _moveDirection = animator.gameObject.transform.forward;
        }
        _moveDirection.y = 0; //Y成分を除く
        animator.gameObject.transform.rotation = Quaternion.LookRotation(_moveDirection); //プレイヤーの向きを変更する
        
        AudioManager.Instance?.PlaySE(7);
        
        
        //指定時間の間補正を行う
        Observable.Timer(TimeSpan.FromSeconds(_correctionStartTime))
            .Subscribe(_ =>
            {
                Observable.IntervalFrame(1) // 毎フレーム処理
                    .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(_correctionEndTime - _correctionStartTime)))
                    .Subscribe(__ =>
                    {
                        animator.gameObject.transform.rotation = Quaternion.LookRotation(_moveDirection);
                        _cc.Move(animator.transform.forward * Time.deltaTime * _correctionSpeed);
                    })
                    .AddTo(_disposables);
            })
            .AddTo(_disposables);
    }

    private void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerMovement.PlayerState.IsSteping = false; //ステップ状態を解除する
    }
}
