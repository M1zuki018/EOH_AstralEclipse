using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using PlayerSystem.ActionFunction;
using PlayerSystem.Input;
using PlayerSystem.Movement;
using PlayerSystem.State;

public class PlayerMovement : MonoBehaviour
{ 
    [Header("コンポーネント")]
    [SerializeField][ReadOnlyOnRuntime] private Transform _playerTransform; // プレイヤーのTransform
    [SerializeField][ReadOnlyOnRuntime] private CinemachineVirtualCamera _playerCamera; // カメラ（任意のカメラ）
    [ReadOnlyOnRuntime] public Animator _animator;
    [SerializeField][ReadOnlyOnRuntime] private CharacterController _characterController;
    
    private PlayerState _playerState;
    public PlayerState PlayerState => _playerState; //公開

    #region 各種機能
    private IMovable _mover; //動き
    private IJumpable _jumper; //ジャンプ
    private IWalkable _walker; //歩きと走り状態の切り替え
    private ICrouchable _croucher; //しゃがみ
    private IInputHandler _inputHandler; //入力情報
    private ISteppable _stepFunction; //ステップ
    private IGaudeable _gaudeFunction; //ガード
    private ILockOnable _lockOnFunction; //ロックオン
    private IWallRunable _wallRunFunction; //ウォールラン
    private IClimbale _climbFunction; //壁のぼり
    private IBigJumpable _bigJumpFunction; //大ジャンプ
    private IVaultable _vaultFunction; //乗り越え
    #endregion
    
    [SerializeField, HighlightIfNull] private WallChecker _wallChecker;

    //未リファクタリング
    private bool _isHanding; //よじのぼり中か
    private bool _isWallRunning; //壁走り中かどうか
    //障害物乗り越え用の変数
    [HideInInspector] public List<Transform> _valutTargetObjects = new List<Transform>();
    
    private void Awake()
    {
        InitializeState();
        InitializeComponents();
    }

    private void InitializeState()
    {
        _playerState = new PlayerState();
    }
    
    private void InitializeComponents()
    {
        //インスタンスを生成
        _mover = new PlayerMover(_characterController, _animator, _playerState, _playerCamera);
        _jumper = (IJumpable) _mover;
        _walker = (IWalkable) _mover;
        _croucher = (ICrouchable) _mover;
        _climbFunction = new ClimbFunction(_animator, _characterController, transform, this);
        
        _inputHandler = new PlayerInputHandler(_playerState, _mover, _jumper, _walker, _croucher,
            GetComponent<StepFunction>(), GetComponent<GaudeFunction>(), GetComponent<LockOnFunction>(),
            GetComponent<WallRunFunction>(), _climbFunction, GetComponent<BigJumpFunction>(),
            GetComponent<VaultFunction>());
        
        _animator.applyRootMotion = true; //ルートモーションを有効化
    }

    #region 入力されたときのメソッド一覧
    
    public void OnAttack(InputAction.CallbackContext context) => _animator.SetTrigger("Attack");
    
    /// <summary>移動処理</summary>
    public void OnMove(InputAction.CallbackContext context) => _inputHandler.HandleMoveInput(context.ReadValue<Vector2>());

    /// <summary>ジャンプ処理</summary>
    public void OnJump(InputAction.CallbackContext context) => HandleJumpInput(context);
    
    /// <summary>歩きと走り状態を切り替える</summary>
    public void OnWalk(InputAction.CallbackContext context) => _inputHandler.HandleWalkInput();
    
    /// <summary>しゃがみ状態を切り替える</summary>
    public void OnCrouch(InputAction.CallbackContext context) => HandleCrouchInput(context);
    
    /// <summary>ステップ</summary>
    public void OnStep(InputAction.CallbackContext context) => _inputHandler.HandleStepInput(); 
    
    /// <summary>ガード状態を切り替える</summary>
    public void OnGuard(InputAction.CallbackContext context) => HandleGuardInput(context);

    /// <summary>ロックオン機能</summary>
    public void OnLockOn(InputAction.CallbackContext context) => _inputHandler.HandleLockOnInput();
    
    /// <summary>パルクールアクションキー</summary>
    public void OnParkourAction(InputAction.CallbackContext context) => HandleParkourAction(context);
    
    #endregion

    #region 入力の条件文

    private void HandleJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed) _inputHandler.HandleJumpInput();
    }

    private void HandleCrouchInput(InputAction.CallbackContext context)
    {
        if (context.performed) _inputHandler.HandleCrouchInput(true);
        if (context.canceled) _inputHandler.HandleCrouchInput(false);
    }

    private void HandleGuardInput(InputAction.CallbackContext context)
    {
        if (context.performed) _inputHandler.HandleGaudeInput(true);
        if (context.canceled) _inputHandler.HandleGaudeInput(false);
    }

    private void HandleParkourAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_playerState.CanVault) //優先１.ヴォルトアクション
                _inputHandler.HandleVaultInput();
            else if (_playerState.CanClimb) //優先２.壁のぼり
                ToggleClimbState();
            else if (_playerState.CanBigJump) //優先３.大ジャンプ
                _inputHandler.HandleBigJumpInput();
        }
    }

    private void ToggleClimbState()
    {
        _playerState.IsClimbing = !_playerState.IsClimbing; //ステートの切り替え
        if (_playerState.IsClimbing) 
            _inputHandler.HandleClimbStartInput();　//壁のぼり開始処理
        else 
            _inputHandler.HandleClimbEndInput(); //壁のぼり終了処理
    }

    #endregion
    
    private void FixedUpdate()
    {
        if (_isHanding) 
        {
            //TODO:崖つかまりの処理
        }
        else if (_playerState.IsClimbing)//壁のぼり中
        {
            _inputHandler.HandleClimbInput(); //壁のぼり処理
        }
        else
        {
            _mover.Move(); //移動処理
            _jumper.Jumping(); //ジャンプ処理
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
            Vector3 velocity = _playerState.Velocity;
            velocity.y = 0f; //地面にいる場合、垂直速度をリセットする
            _playerState.Velocity = velocity;
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
}