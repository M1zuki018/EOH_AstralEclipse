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
    
    [Header("キャラクター設定")]
    [SerializeField, Comment("ジャンプの高さ")] private float _jumpPower = 5f;
    [SerializeField, Comment("重力")] private float _gravity = -9.81f;
    [SerializeField, Comment("回転速度")] private float _rotationSpeed = 10f;
    [SerializeField, Comment("壁を登る速さ")] private float _climbSpeed = 3f;

    #region 各種機能
    private IMovable _mover;
    private IJumpable _jumper;
    private IWalkable _walker;
    private ICrouchable _croucher;
    private IInputHandler _inputHandler;
    private ISteppable _stepFunction; 
    private IGaudeable _gaudeFunction;
    private ILockOnable _lockOnFunction; 
    private IWallRunable _wallRunFunction;
    private IVaultable _vaultFunction;
    private IBigJumpable _bigJumpFunction;
    private IClimbale _climbFunction;
    #endregion
    
    [SerializeField, HighlightIfNull] private WallChecker _wallChecker;

    private bool _isHanding; //よじのぼり中か
    
    //壁走り用の変数
    private bool _isWallRunning; //壁走り中かどうか
    
    //障害物乗り越え用の変数
    [HideInInspector] public List<Transform> _valutTargetObjects = new List<Transform>();
    
    private void Awake()
    {
        _stepFunction = GetComponent<StepFunction>();
        _gaudeFunction = GetComponent<GaudeFunction>();
        _lockOnFunction = GetComponent<LockOnFunction>();
        _wallRunFunction = GetComponent<WallRunFunction>();
        _bigJumpFunction = GetComponent<BigJumpFunction>();
        _vaultFunction = GetComponent<VaultFunction>();
        _climbFunction = new ClimbFunction(_animator, _characterController, transform, _climbSpeed, this);
        
        //インスタンスを生成
        _playerState = new PlayerState();
        _mover = new PlayerMover(_characterController, _animator, _playerState, _playerCamera);
        _jumper = (IJumpable) _mover;
        _walker = (IWalkable) _mover;
        _croucher = (ICrouchable) _mover;
        
        _inputHandler = new PlayerInputHandler(_playerState, _mover, _jumper, _walker, _croucher, 
            _stepFunction, _gaudeFunction, _lockOnFunction, _wallRunFunction, _climbFunction, _bigJumpFunction, _vaultFunction);
        
        _animator.applyRootMotion = true; //ルートモーションを有効化
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _animator.SetTrigger("Idle");
    }
    
    /// <summary>移動処理</summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context) => _inputHandler.HandleMoveInput(context.ReadValue<Vector2>());

    /// <summary>ジャンプ処理</summary>
    public void OnJump(InputAction.CallbackContext context) { if (context.performed) _inputHandler.HandleJumpInput(); }
    
    /// <summary>歩きと走り状態を切り替える</summary>
    public void OnWalk(InputAction.CallbackContext context) { if (context.performed) _inputHandler.HandleWalkInput(); }
    
    /// <summary>しゃがみ状態を切り替える</summary>
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed) _inputHandler.HandleCrouchInput(true); //ボタンが押されたとき
        if (context.canceled) _inputHandler.HandleCrouchInput(false); //ボタンが放されたとき
    }
    
    /// <summary>ステップ</summary>
    public void OnStep(InputAction.CallbackContext context) { if (context.performed) _inputHandler.HandleStepInput(); }
    
    /// <summary>ガード状態を切り替える</summary>
    public void OnGuard(InputAction.CallbackContext context)
    {
        if (context.performed) _inputHandler.HandleGaudeInput(true); //ボタンが押されたとき
        if (context.canceled) _inputHandler.HandleGaudeInput(false); //ボタンが放されたとき
    }

    /// <summary>ロックオン機能</summary>
    public void OnLockOn(InputAction.CallbackContext context) { if (context.performed) _inputHandler.HandleLockOnInput(); }

    /// <summary>
    /// パルクールアクションキー
    /// </summary>
    public void OnParkourAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_playerState.CanVault) //優先１.ヴォルトアクション
            {
                _inputHandler.HandleVaultInput();
            }
            else if (_playerState.CanClimb) //優先２.壁のぼり
            {
                _playerState.IsClimbing = !_playerState.IsClimbing; //ステートの切り替え
                
                if (_playerState.IsClimbing) //壁のぼり開始なら
                {
                    _playerState.IsJumping = false; //ジャンプの途中で壁を掴んだ時、ジャンプフラグをオフにする
                    _climbFunction.StartClimbing();
                }
                else //壁のぼり終了なら
                {
                    _climbFunction.EndClimbing();
                }
            }
            else if (_playerState.CanBigJump) //優先３.大ジャンプ
            {
                _inputHandler.HandleBigJumpInput();
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (_isHanding) 
        {
            //
        }
        else if (_playerState.IsClimbing)//壁のぼり中
        {
            _climbFunction.HandleClimbing(PlayerState.MoveDirection);
        }
        else
        {
            _mover.Move();
            _jumper.Jumping();
            HandleGroundedCheck();
            HandleFalling();
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
        //接地判定はfalseだが、落下中と判定しない例外
        //ジャンプ中/壁登り中/乗り越え中
        if (!_playerState.IsJumping && !_playerState.IsClimbing && !_playerState.IsVaulting)
        {
            if (_playerState.IsGrounded)
            {
                _animator.SetBool("IsFalling", false);
            }
            else
            {
                _animator.SetBool("IsFalling", true);
            }
        }
    }
}