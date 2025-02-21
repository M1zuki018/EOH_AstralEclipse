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
using PlayerInputManager = PlayerSystem.Input.PlayerInputManager;
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

    protected override void Awake()
    {
        base.Awake();

        _playerBlackBoard = new PlayerBlackBoard(_data, _status, GetComponent<PlayerInputManager>());
    }
    
    private void Start()
    {
        _playerController = GetComponent<PlayerController>(); //Animator、State取得用
        _playerStateMachine = new PlayerStateMachine(
            inputProcessor: GetComponent<PlayerInputManager>().IPlayerInputReceiver as PlayerInputProcessor,
            blackboard: _playerBlackBoard,
            actionHandler: _playerController.PlayerActionHandler);
    }

    private void Update() => _playerStateMachine.Update();
    private void FixedUpdate() => _playerStateMachine.FixedUpdate();

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
            _playerController.AnimationController.Common.PlayDamageAnimation();
        }
    }

    protected override async void HandleDeath(GameObject attacker)
    {
        _playerController.AnimationController.Common.PlayDeathAnimation();
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
