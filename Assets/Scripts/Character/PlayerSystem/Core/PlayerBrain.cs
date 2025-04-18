using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayerSystem.Core;
using PlayerSystem.Input;
using PlayerSystem.State;
using UnityEngine;
using PlayerInputManager = PlayerSystem.Input.PlayerInputManager;

/// <summary>
/// プレイヤーの中心となるクラス（体力・死亡管理）
/// </summary>
[RequireComponent(typeof(PlayerController), typeof(Health), typeof(PlayerCombat))]
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerBrain : CharacterBase
{
    private PlayerController _controller;
    
    // 黒板登録用
    [SerializeField] private PlayerDataSO _data;
    [SerializeField] private PlayerStatusSO _status;
    [SerializeField] private PlayerSettingsSO _settings;
    
    // 実行中に変更されるデータを格納した黒板
    private PlayerBlackBoard _bb;
    public PlayerBlackBoard BB => _bb;
    
    // ステートマシン
    private PlayerStateMachine _stateMachine;
    public PlayerStateMachine StateMachine => _stateMachine;
    
    // 補助クラス
    private PlayerUIController _uiController;
    private PlayerCameraController _cameraController;
    private PlayerAudioController _audioController;
    
    public override UniTask OnAwake()
    {
        _controller = GetComponent<PlayerController>(); // Animator、State取得用
        _bb = new PlayerBlackBoard(_data, _status, _settings, GetComponent<PlayerInputManager>());
        
        // 補助クラスのインスタンスを作成
        _uiController = new PlayerUIController(_bb);
        _cameraController = new PlayerCameraController();
        _audioController = new PlayerAudioController();
        
        return base.OnAwake();
    }
    
    public override UniTask OnStart()
    {
        // ステートマシン作成
        _stateMachine = new PlayerStateMachine(
            inputProcessor: GetComponent<PlayerInputManager>().IPlayerInputReceiver as PlayerInputProcessor,
            blackboard: _bb,
            actionHandler: _controller.PlayerActionHandler);
        
        _uiController.Initialized(); // 初期化
        
        return base.OnStart();
    }
    
    private void Update() => _stateMachine.Update();
    private void FixedUpdate() => _stateMachine.FixedUpdate();

    /// <summary>
    /// ダメージを喰らった時の処理
    /// </summary>
    protected override void HandleDamage(int damage, GameObject attacker)
    {
        Debug.Log($"{attacker.name}から{damage}ダメージ受けた！！");
        _uiController.ShowDamage(damage, transform);

        if (_bb.IsGuarding)
        {
            // ガード中はWillを減らす処理を行う
            _uiController.UpdateWill();
        }
        else
        {
            // HPを減らす処理、カメラシェイク、ダメージSE
            _uiController.UpdateHP();
            _cameraController.Shake();
            _audioController.HitSE();
        
            if (!_health.IsDead)
            {
                //死亡していないときだけダメージアニメーションをトリガー
                _bb.AnimController.Common.PlayDamageAnimation();
            }
        }
    }

    /// <summary>
    /// 死亡時の処理
    /// </summary>
    protected override async void HandleDeath(GameObject attacker)
    {
        _bb.AnimController.Common.PlayDeathAnimation(); // アニメーション
        //_playerInput.DeactivateInput(); //入力制限
        _audioController.FadeOut();
        _uiController.WhenDeath(); //UI処理
        _cameraController.WhenDeath();
        
        await UniTask.Delay(3000);
        
        _uiController.ShowDeathPanel(); // 死亡時のパネルを非表示にする
        
        //スローモーションにする
        Time.timeScale = 0.3f; 
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0f, 2f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true) // TimeScaleの影響を受けずにアニメーションする
            .OnComplete(() => Debug.Log("完全停止"));
    }
}
