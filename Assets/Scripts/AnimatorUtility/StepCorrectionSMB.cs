using UniRx;
using UnityEngine;
using System;

/// <summary>
/// ステップモーションに補正をかけるクラス
/// </summary>
public class StepCorrectionSMB : StateMachineBehaviour
{
    [SerializeField, Comment("補正をはじめる時間")] private float _correctionStartTime = 0.2f;
    [SerializeField, Comment("補正を終える時間")] private float _correctionEndTime = 0.6f;
    [SerializeField, Comment("移動スピード")] private float _correctionSpeed = 3.0f;
    
    private Vector3 _moveDirection;
    private CharacterController _cc;
    private PlayerBrain _brain;
    
    private CompositeDisposable _disposables = new CompositeDisposable();


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_cc == null)_cc = animator.GetComponent<CharacterController>();
        if(_brain == null)_brain = animator.GetComponent<PlayerBrain>();

        _brain.BB.IsSteping = true; //ステップ状態にする

        Vector3 direction = _brain.BB.CorrectedDirection.normalized;
        
        if (direction.magnitude > 0.05f)
        {
            //入力があったらプレイヤーの入力を保存しておく
            _moveDirection = direction;
        }
        else
        {
            //入力がなかったらキャラクターの正面方向を保存しておく
            _moveDirection = animator.gameObject.transform.forward;
        }
        
        _moveDirection.y = 0; //Y成分を除く
        animator.gameObject.transform.rotation = Quaternion.LookRotation(_moveDirection); //プレイヤーの向きを変更する

        CameraManager.Instance?.StepEffect();
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
                        _cc.Move(animator.transform.forward * Time.deltaTime * _correctionSpeed); // TODO:ここでマイナスをかけてうしろに行くようにする
                    })
                    .AddTo(_disposables);
            })
            .AddTo(_disposables);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CameraManager.Instance?.EndStepEffect();
        _brain.BB.IsSteping = false; //ステップ状態を解除する
    }
}
