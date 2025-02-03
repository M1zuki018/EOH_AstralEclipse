using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    private PlayerInput _playerInput;
    
    //Idleモーション再生のための変数
    [SerializeField] private float _idleThreshold = 5f; //無操作とみなす秒数
    [SerializeField, HighlightIfNull] private List<InputActionReference> _moveActions; //InputSystemのアクション参照
    [SerializeField, HighlightIfNull] private InputActionReference _lookAction; //カメラ操作のアクション参照
    private Subject<Unit> _inputDetected = new Subject<Unit>();
    
    private async void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>(); //Animator、State取得用
        _playerInput = GetComponent<PlayerInput>();

        if (!_playerMovement.PlayerState.DebugMode)
        {
            //開始演出
            _playerInput.DeactivateInput();
            SkinManager.Instance?.ChangeSkin(0);
            CameraManager.Instance?.UseCamera(3);
            UIManager.Instance?.InitializePlayerHP(GetMaxHP(), GetCurrentHP());
            UIManager.Instance?.HideRightUI();
            UIManager.Instance?.HideFirstText();
        }
        
        //TODO: 最初からモーションを流せるように変更する
        SubscribeToInputEvents(); //入力イベントを購読
        
        _inputDetected
            .Throttle(TimeSpan.FromSeconds(_idleThreshold)) //最後の入力から指定した間入力がなかったら以下の処理を行う
            .Subscribe(_ => PlayRandomIdleMotion())
            .AddTo(this);

        await UniTask.Delay(2700);
        
        //操作開始
        CameraManager.Instance?.UseCamera(0);
        
        await UniTask.Delay(1200);
        
        UIManager.Instance?.ShowFirstText(); //最初のクエスト説明を表示
        _moveActions[1].action.Enable(); //有効化
        
        // ボタンが押されたら入力を有効化
        Observable.FromEvent<InputAction.CallbackContext>(
                h => _moveActions[1].action.performed += h,
                h => _moveActions[1].action.performed -= h)
            .Take(1) // 最初の1回だけ
            .Subscribe(GameStart)
            .AddTo(this);
        
    }

    private void GameStart(InputAction.CallbackContext context)
    {
        Debug.Log("Game started");
        UIManager.Instance?.HideFirstText();
        UIManager.Instance?.ShowRightUI();
        _playerInput.ActivateInput();
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
        Debug.Log($"{attacker.name}から{damage}ダメージ受けた！！");
        UIManager.Instance?.UpdatePlayerHP(GetCurrentHP());
        _playerMovement._animator.SetTrigger("Damage");
        CameraManager.Instance?.TriggerCameraShake();
    }

    protected override void HandleDeath(GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}に倒された！");
        _playerMovement._animator.SetTrigger("Damage");
        //TODO:死亡エフェクト等の処理
    }

    [ContextMenu("Shake")]
    public void Shake()
    {
        CameraManager.Instance?.TriggerCameraShake();
    }
    
}
