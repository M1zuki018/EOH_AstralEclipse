using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using PlayerSystem.ActionFunction;
using PlayerSystem.Input;
using PlayerSystem.Movement;
using PlayerSystem.State;

/// <summary>
/// プレイヤーの移動機能
/// </summary>
public class PlayerMovement : MonoBehaviour, IMatchTarget
{ 
    [Header("コンポーネント")]
    [SerializeField][ReadOnlyOnRuntime] private Transform _playerTransform; // プレイヤーのTransform
    [SerializeField][ReadOnlyOnRuntime] private CinemachineVirtualCamera _playerCamera; // カメラ（任意のカメラ）
    [ReadOnlyOnRuntime] public Animator _animator;
    [SerializeField][ReadOnlyOnRuntime] private CharacterController _characterController;
    
    private PlayerState _playerState;
    public PlayerState PlayerState => _playerState; //公開
    
    public InteractableItemBase InteractableItem { get; set; } //インタラクト可能なアイテム

    private Collider _collider;
    [SerializeField] private Transform _targetTransform;
    
    #region 各種機能
    private IInputHandler _inputHandler; //入力情報
    private IMovable _mover; //移動
    private IJumpable _jumper; //ジャンプ
    private IWalkable _walker; //歩きと走り状態の切り替え
    private ISteppable _stepFunction; //ステップ
    private IGaudeable _gaudeFunction; //ガード
    private ILockOnable _lockOnFunction; //ロックオン
    #endregion
    
    private void Awake()
    {
        InitializeState();
        InitializeComponents();
        
        TryGetComponent(out _collider);
        _animator.keepAnimatorStateOnDisable = true;
        foreach (var smb in _animator.GetBehaviours<MatchPositionSMB>())
        {
            smb._target = this;
        }
    }

    private void InitializeState()
    {
        _playerState = new PlayerState();
    }
    
    private void InitializeComponents()
    {
        //インスタンスを生成
        _mover = new PlayerMover(_characterController, _animator, _playerState, _playerCamera, GetComponent<TrailRenderer>());
        _jumper = (IJumpable) _mover;
        _walker = (IWalkable) _mover;
        
        _inputHandler = new PlayerInputHandler(this, _playerState, _mover, _jumper, _walker,
            GetComponent<StepFunction>(), GetComponent<GaudeFunction>(), GetComponent<LockOnFunction>(),
             GetComponent<PlayerCombat>());
        
        _animator.applyRootMotion = true; //ルートモーションを有効化
    }

    #region 入力されたときのメソッド一覧
    
    /// <summary>攻撃処理</summary>
    public void OnAttack(InputAction.CallbackContext context) => HandleAttackInput(context);

    /// <summary>スキル処理</summary>
    public void OnSkill1(InputAction.CallbackContext context) => HandleSkillInput(context, 1); 
    public void OnSkill2(InputAction.CallbackContext context) => HandleSkillInput(context, 2);
    public void OnSkill3(InputAction.CallbackContext context) => HandleSkillInput(context, 3);
    public void OnSkill4(InputAction.CallbackContext context) => HandleSkillInput(context, 4);
    
    /// <summary>移動処理</summary>
    public void OnMove(InputAction.CallbackContext context) => _inputHandler.HandleMoveInput(context.ReadValue<Vector2>());

    /// <summary>ジャンプ処理</summary>
    public void OnJump(InputAction.CallbackContext context) => HandleJumpInput(context);
    
    /// <summary>歩きと走り状態を切り替える</summary>
    public void OnWalk(InputAction.CallbackContext context) => _inputHandler.HandleWalkInput();
    
    /// <summary>ステップ</summary>
    public void OnStep(InputAction.CallbackContext context) => HandleStepInput(context);
    
    /// <summary>ガード状態を切り替える</summary>
    public void OnGuard(InputAction.CallbackContext context) => HandleGuardInput(context);

    /// <summary>ロックオン機能</summary>
    public void OnLockOn(InputAction.CallbackContext context) => HandleLockOnInput(context);

    public void OnPause(InputAction.CallbackContext context) => _inputHandler.HandlePauseInput();
    
    public void OnNone(InputAction.CallbackContext context) => Debug.Log("登録されていないボタンです");

    #endregion

    #region 入力の条件文

    private void HandleAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed) _inputHandler.HandleAttackInput();
    }
    
    private void HandleSkillInput(InputAction.CallbackContext context, int index)
    {
        //index で スキル1~4のどのボタンを押されたか判断する
        if (context.performed) _inputHandler.HandleSkillInput(index);
    }
    private void HandleJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed) _inputHandler.HandleJumpInput();
    }

    private void HandleStepInput(InputAction.CallbackContext context)
    {
        if (context.performed) _inputHandler.HandleStepInput();
    }

    private void HandleLockOnInput(InputAction.CallbackContext context)
    {
        if (context.performed) _inputHandler.HandleLockOnInput();
    }

    private void HandleGuardInput(InputAction.CallbackContext context)
    {
        if (context.performed) _inputHandler.HandleGaudeInput(true);
        if (context.canceled) _inputHandler.HandleGaudeInput(false);
    }

    #endregion
    
    private void FixedUpdate()
    {
        
        if (_playerState.IsJumping)
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
        if (_playerState.IsGrounded && _playerState.Velocity.y < 0)
        {
            _playerState.IsJumping = false;
            _playerState.Velocity = new Vector3(0, -0.1f, 0); //確実に地面につくように少し下向きの力を加える
            _animator.SetBool("IsJumping", false);
            _animator.applyRootMotion = true;
        }
    }

    /// <summary>
    /// 落下中かどうかの判定を行う
    /// </summary>
    private void HandleFalling()
    {
        _animator.SetBool("IsGround", _playerState.IsGrounded);
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