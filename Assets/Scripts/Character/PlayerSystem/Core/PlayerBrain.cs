using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

/// <summary>
/// プレイヤーの中心となるクラス（体力・死亡管理）
/// </summary>
[RequireComponent(typeof(PlayerMovement), typeof(Health), typeof(PlayerCombat))]
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerBrain : CharacterBase
{
    private PlayerMovement _playerMovement;
    
    //Idleモーション再生のための変数
    [SerializeField] private float _idleThreshold = 5f; //無操作とみなす秒数
    [SerializeField, HighlightIfNull] private List<InputActionReference> _moveActions; //InputSystemのアクション参照
    [SerializeField, HighlightIfNull] private InputActionReference _lookAction; //カメラ操作のアクション参照
    private Subject<Unit> _inputDetected = new Subject<Unit>();
    
    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>(); //Animator、State取得用
        _uiManager.InitializePlayerHP(GetMaxHP(), GetCurrentHP());
        
        //TODO: 最初からモーションを流せるように変更する
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
                _playerMovement._animator.SetBool("BackToIdle", true); //Idleモーションを中断
            })
            .AddTo(this);
    }

    /// <summary>
    /// アイドルモーションの抽選・再生を行う
    /// </summary>
    private void PlayRandomIdleMotion()
    {
        int rand = Random.Range(0, 2); //モーションの抽選
        _playerMovement._animator.SetBool("BackToIdle", false); //falseに戻しておく
        _playerMovement._animator.SetInteger("IdleType", rand);
        _playerMovement._animator.SetTrigger("PlayIdle");
    }

    protected override void HandleDamage(int damage, GameObject attacker)
    {
        DebugOverlay.Log($"{attacker.name}から{damage}ダメージ受けた！！");
        Debug.Log($"{attacker.name}から{damage}ダメージ受けた！！");
        _uiManager.UpdatePlayerHP(GetCurrentHP());
    }

    protected override void HandleDeath(GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}に倒された！");
        //TODO:死亡エフェクト等の処理
    }
}
