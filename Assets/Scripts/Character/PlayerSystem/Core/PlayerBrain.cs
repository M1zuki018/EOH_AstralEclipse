using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayerSystem.Core;
using PlayerSystem.Input;
using PlayerSystem.State;
using PlayerSystem.State.Base;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

/// <summary>
/// プレイヤーの中心となるクラス（体力・死亡管理）
/// </summary>
[RequireComponent(typeof(PlayerController), typeof(Health), typeof(PlayerCombat))]
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerBrain : CharacterBase
{
    private PlayerController _playerController;
    private PlayerStateMachine _playerStateMachine;

    // 黒板登録用
    [SerializeField] private PlayerDataSO _data;
    [SerializeField] private PlayerStatusSO _status;
    
    // 実行中に変更されるデータを格納した黒板
    private PlayerBlackBoard _playerBlackBoard;
    public PlayerBlackBoard BB => _playerBlackBoard;
    
    //Idleモーション再生のための変数
    [SerializeField] private float _idleThreshold = 5f; //無操作とみなす秒数
    [SerializeField, HighlightIfNull] private InputActionReference _lookAction; //カメラ操作のアクション参照
    private Subject<Unit> _inputDetected = new Subject<Unit>();

    protected override void Awake()
    {
        base.Awake();

        _playerBlackBoard = new PlayerBlackBoard(_data, _status);
    }
    
    private void Start()
    {
        _playerController = GetComponent<PlayerController>(); //Animator、State取得用
        _playerStateMachine = new PlayerStateMachine(
            inputProcessor: GetComponent<PlayerSystem.Input.PlayerInputManager>().IPlayerInputReceiver as PlayerInputProcessor,
            blackboard: _playerBlackBoard,
            actionHandler: _playerController.PlayerActionHandler);
    }

    private void Update() => _playerStateMachine.Update();
    private void FixedUpdate() => _playerStateMachine.FixedUpdate();


    private void Tmp()
    {
        SubscribeToInputEvents(); //入力イベントを購読
        
        _inputDetected
            .Throttle(TimeSpan.FromSeconds(_idleThreshold)) //最後の入力から指定した間入力がなかったら以下の処理を行う
            .Subscribe(_ => PlayRandomIdleMotion())
            .AddTo(this);
    }
    
    /// <summary>
    /// プレイヤーの入力を監視する
    /// </summary>
    private void SubscribeToInputEvents()
    {
        /*
        // 全てのアクションからObservableを作成
        var moveActionStreams = new List<IObservable<InputAction.CallbackContext>>();
        foreach (var action in _moveActions)
        {
            moveActionStreams.Add(action.action.PerformedAsObservable()); //InputActionをObservableに変換する
        }

        // 全てのアクションをマージして監視
        moveActionStreams
            .Merge() // Observableを1つに統合
            .Subscribe(_ =>
            {
                _inputDetected.OnNext(Unit.Default); // 入力があった時、通知を行う
                _playerController.Animator.SetBool("BackToIdle", true); //Idleモーションを中断
            })
            .AddTo(this);
            */
    }

    /// <summary>
    /// アイドルモーションの抽選・再生を行う
    /// </summary>
    private void PlayRandomIdleMotion()
    {
        int rand = Random.Range(0, 2); //モーションの抽選
        _playerController.Animator.SetBool("BackToIdle", false); //falseに戻しておく
        _playerController.Animator.SetInteger("IdleType", rand);
        _playerController.Animator.SetTrigger("PlayIdle");
    }

    protected override void HandleDamage(int damage, GameObject attacker)
    {
        Debug.Log($"{attacker.name}から{damage}ダメージ受けた！！");
        UIManager.Instance?.ShowDamageAmount(damage, transform);
        UIManager.Instance?.UpdatePlayerHP(GetCurrentHP());
        CameraManager.Instance?.TriggerCameraShake(); //カメラを揺らす
        AudioManager.Instance?.PlaySE(14); //ヒット時のSE
        
        if (!_health.IsDead)
        {
            //死亡していないときだけダメージアニメーションをトリガー
            _playerController.Animator.SetTrigger("Damage");
        }
    }

    protected override async void HandleDeath(GameObject attacker)
    {
        _playerController.Animator.SetTrigger("IsDeath");
        //_playerInput.DeactivateInput(); //入力制限
        //TODO:死亡エフェクト等の処理

        AudioManager.Instance.FadeOut(AudioType.BGM);
        AudioManager.Instance.FadeOut(AudioType.SE);
        
        //UI処理
        UIManager.Instance.HidePlayerBattleUI();
        UIManager.Instance.HideRightUI();
        UIManager.Instance.HideLockOnUI();
        UIManager.Instance.HideBossUI();
        
        CameraManager.Instance.PlayerDeath();
        
        await UniTask.Delay(3000);
        
        UIManager.Instance.ShowDeathPanel();
        
        //スローモーションにする
        Time.timeScale = 0.3f; 
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0f, 2f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true) // TimeScaleの影響を受けずにアニメーションする
            .OnComplete(() => Debug.Log("完全停止"));
        
    }
}
