using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    
    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>(); //Animator、State取得用
        _playerInput = GetComponent<PlayerInput>();

        _playerInput.DeactivateInput(); //入力を受け付けない
        CameraManager.Instance.UseCamera(3); //プレイヤー正面のカメラを使う
        
        GameManager.Instance.OnPlay += StartPerformance;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.Instance.OnPlay -= StartPerformance;
    }

    /// <summary>
    /// 開始演出中の処理
    /// </summary>
    private async void StartPerformance()
    {
        if (!_playerMovement.PlayerState.DebugMode)
        {
            _playerInput.DeactivateInput();
            UIManager.Instance?.InitializePlayerHP(GetMaxHP(), GetCurrentHP());   
            
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
    }

    private async void GameStart(InputAction.CallbackContext context)
    {
        Debug.Log("Game started");
        AudioManager.Instance?.PlaySE(9);
        UIManager.Instance?.ShowStartText();
        UIManager.Instance?.HideFirstText();
        UIManager.Instance?.ShowRightUI();
        _playerInput.ActivateInput();
        
        await UniTask.Delay(500);
        
        UIManager.Instance.HideStartText(); //「GameStart」の文字を非表示にする
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
        UIManager.Instance?.ShowDamageAmount(damage, transform);
        UIManager.Instance?.UpdatePlayerHP(GetCurrentHP());
        CameraManager.Instance?.TriggerCameraShake(); //カメラを揺らす
        
        if (!_health.IsDead)
        {
            //死亡していないときだけダメージアニメーションをトリガー
            _playerMovement._animator.SetTrigger("Damage");
        }
    }

    protected override async void HandleDeath(GameObject attacker)
    {
        _playerMovement._animator.SetTrigger("IsDeath");
        _playerInput.DeactivateInput(); //入力制限
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

    [ContextMenu("Shake")]
    public void Shake()
    {
        CameraManager.Instance?.TriggerCameraShake();
    }
    
}
