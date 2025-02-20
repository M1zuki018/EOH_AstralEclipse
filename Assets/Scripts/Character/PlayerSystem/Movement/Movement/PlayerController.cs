using System;
using UnityEngine;
using Cinemachine;
using PlayerSystem.ActionFunction;
using PlayerSystem.Fight;
using PlayerSystem.Input;
using PlayerSystem.Movement;
using PlayerSystem.State;

/// <summary>
/// プレイヤーの移動・空中移動などの処理を行う機能
/// </summary>
public class PlayerController : MonoBehaviour, IMatchTarget
{ 
    [Header("コンポーネント")]
    [SerializeField][ReadOnlyOnRuntime] private Transform _playerTransform; // プレイヤーのTransform
    [SerializeField][ReadOnlyOnRuntime] private CinemachineVirtualCamera _playerCamera; // カメラ
    [SerializeField][ReadOnlyOnRuntime] private CharacterController _characterController;
    [SerializeField][ReadOnlyOnRuntime] private PlayerBrain _playerBrain;
    
    // Animator
    [SerializeField][ReadOnlyOnRuntime] private Animator _animator;
    public Animator Animator => _animator;
    
    // ステートマシンと処理をつなぐ
    private PlayerActionHandler _playerActionHandler;
    public PlayerActionHandler PlayerActionHandler => _playerActionHandler;

    private Collider _collider;
    [SerializeField] private Transform _targetTransform;
    
    #region 各種機能
    private IMovable _mover; //移動
    private IJumpable _jumper; //ジャンプ
    private IWalkable _walker; //歩きと走り状態の切り替え
    private ISteppable _stepFunction; //ステップ
    private IGaudeable _gaudeFunction; //ガード
    private ILockOnable _lockOnFunction; //ロックオン
    #endregion
    
    private void Awake()
    {
        InitializeComponents();
        
        TryGetComponent(out _collider);
        Animator.keepAnimatorStateOnDisable = true;
        
        // ターゲットマッチングを行うStateMachineBehaviorに自身を登録する
        foreach (var smb in Animator.GetBehaviours<MatchPositionSMB>())
        {
            smb._target = this;
        }
        
        _walker.Walk(); // 移動速度切り替えのObservableを購読する
    }
    
    private void InitializeComponents()
    {
        // 移動処理を包括したクラスのインスタンスを生成
        _mover = new PlayerControlFunction(
            new PlayerMovement(_playerBrain.BB, _characterController, Animator,  _playerCamera, GetComponent<TrailRenderer>()),
            new PlayerJump(_playerBrain.BB, _characterController, Animator,  _playerCamera, GetComponent<TrailRenderer>()),
            _characterController, Animator, _playerBrain.BB, _playerCamera, GetComponent<TrailRenderer>());
        _jumper = (IJumpable) _mover;
        _walker = (IWalkable) _mover;

        _playerActionHandler = new PlayerActionHandler(
            mover: _mover,
            jumper: _jumper,
            walker: _walker,
            steppable: _stepFunction,
            gauder: _gaudeFunction,
            locker: _lockOnFunction,
            combat: GetComponent<PlayerCombat>());
        
        Animator.applyRootMotion = true; //ルートモーションを有効化
    }

    private void OnDestroy()
    {
        _walker.DisposeWalkSubscription(); // 移動速度切り替えのObservableを購読解除
    }

    private void FixedUpdate()
    {
        if (_playerBrain.BB.IsJumping)
        {
            _jumper.Jumping(); //ジャンプ処理
            HandleGroundedCheck();
            HandleFalling(); //落下中の判定
        }
        else
        {
            _mover.Move(); //移動処理
            HandleGroundedCheck();
            HandleFalling(); //落下中の判定
        }
    }
    
    /// <summary>
    /// 地面にいるときの処理
    /// </summary>
    private void HandleGroundedCheck()
    {
        if (_playerBrain.BB.IsGrounded && _playerBrain.BB.Velocity.y < 0)
        {
            _playerBrain.BB.IsJumping = false;
            _playerBrain.BB.Velocity = new Vector3(0, -0.1f, 0); //確実に地面につくように少し下向きの力を加える
            Animator.SetBool("IsJumping", false);
            Animator.applyRootMotion = true;
        }
    }

    /// <summary>
    /// 落下中かどうかの判定を行う
    /// </summary>
    private void HandleFalling()
    {
        Animator.SetBool("IsGround", _playerBrain.BB.IsGrounded);
        /*
        //接地判定はfalseだが、落下中と判定しない例外
        //ジャンプ中/壁登り中/乗り越え中
        if (!_playerState.IsJumping && !_playerState.IsClimbing && !_playerState.IsVaulting)
        {
            _animator.SetBool("IsGround", !_playerState.IsGrounded);
        }
        */
    }

    public Vector3 TargetPosition => _collider.ClosestPoint(_targetTransform.position);
    
    /// <summary>アニメーションイベントでSEを再生するためのメソッド</summary>
    public void PlaySE(int index) => AudioManager.Instance?.PlaySE(index);
}