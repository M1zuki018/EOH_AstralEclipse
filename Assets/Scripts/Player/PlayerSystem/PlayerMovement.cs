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
    private CharacterController _characterController;
    
    private PlayerState _playerState;
    private IMovable _mover;
    private IJumpable _jumper;
    private IWalkable _walker;
    private ICrouchable _croucher;
    private IInputHandler _inputHandler;
    
    [Header("キャラクター設定")]
    [SerializeField, Comment("ジャンプの高さ")] private float _jumpPower = 5f;
    [SerializeField, Comment("重力")] private float _gravity = -9.81f;
    [SerializeField, Comment("回転速度")] private float _rotationSpeed = 10f;
    [SerializeField, Comment("壁を登る速さ")] private float _climbSpeed = 3f;
    
    [Header("各種機能")]
    private ISteppable _stepFunction; 
    private IGaudeable _gaudeFunction;
    private ILockOnable _lockOnFunction; 
    private IWallRunable _wallRunFunction;
    private IVaultable _vaultFunction;
    private IBigJumpable _bigJumpFunction;
    private IClimbale _climbFunction;
    [SerializeField, HighlightIfNull] private WallChecker _wallChecker;
    
    private Vector3 _moveDirection; // 入力された方向
    private Vector3 _velocity; //垂直方向の速度
    private float _moveSpeed; // 移動する速度

    private bool _isGround; //地面についているか
    private bool _isCrouching; //しゃがんでいるか
    private bool _isJumping; //ジャンプ中か
    private bool _isHanding; //よじのぼり中か
    
    //壁のぼり用の変数
    private Vector3 _wallNormal; // 壁の法線
    private Vector3 _climbDirectionParallel; // 壁に沿った平行方向
    private Vector3 _climbDirectionUp; // 壁に沿った上下方向
    private bool _isClimbing; //壁のぼり中か
    private bool _canClimb; //壁のぼりできるか
    private bool _isClimbingStopped; //壁に掴まった状態の停止フラグ
    
    //壁走り用の変数
    private bool _isWallRunning; //壁走り中かどうか
    
    //障害物乗り越え用の変数
    [HideInInspector] public List<Transform> _valutTargetObjects = new List<Transform>();
    public bool IsVault { get; set; }
    
    public bool IsGround
    {
        get { return _isGround;}
        set => _isGround = value; }
    public bool IsWall { get; set; }
    
    public bool CanClimb { get; set; }
    public bool IsClimbing { get {return _isClimbing;} }
    public Vector3 WallNormal { set => _wallNormal = value; }
    
    /// <summary>ヴォルトできるか</summary>
    public bool CanVault { get; set; }
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _stepFunction = GetComponent<StepFunction>();
        _gaudeFunction = GetComponent<GaudeFunction>();
        _lockOnFunction = GetComponent<LockOnFunction>();
        _wallRunFunction = GetComponent<WallRunFunction>();
        _bigJumpFunction = GetComponent<BigJumpFunction>();
        _vaultFunction = GetComponent<VaultFunction>();
        _climbFunction = GetComponent<ClimbFunction>();
        
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
                _playerState.IsClimbing = !_playerState.IsClimbing;
                
                if (_playerState.IsClimbing) //壁のぼり開始なら
                {
                    _playerState.IsJumping = false; //ジャンプの途中で壁を掴んだ時、ジャンプフラグをオフにする
                    _animator.SetTrigger("Climb");
                    _animator.SetBool("IsClimbing", true);
                    _animator.applyRootMotion = false; //ルートモーションを使用しない
                    CalculateClimbDirections(); //壁の法線に基づいて方向を計算
                }
                else //壁のぼり終了なら
                {
                    _animator.SetBool("IsClimbing", false);
                    _isClimbingStopped = false; //停止フラグをリセット
                    //_animator.applyRootMotion = true;
                    _playerState.IsGrounded = false;
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
            HandleClimbing();
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
    
    
    /// <summary>
    /// 壁のぼり処理
    /// </summary>
    private void HandleClimbing()
    {
        if (_wallChecker.IsGrab)
        {
            //動きを停止する
            _animator.SetFloat("ClimbSpeed",0);
            if (_moveDirection.sqrMagnitude > 0.01f)
            {
                _animator.applyRootMotion = true;
                _isHanding = true;
                _animator.SetTrigger("Hang");
            }
        }
        else
        {
            if (_moveDirection.sqrMagnitude > 0.01f)
            {
                Vector3 climbDirection = Quaternion.LookRotation(-_wallNormal) * _moveDirection;
                _characterController.Move(climbDirection * _climbSpeed * Time.deltaTime);
        
                _animator.SetFloat("ClimbSpeed", climbDirection.magnitude, 0.1f, Time.deltaTime);
            }
            else
            {
                //停止中の処理
                _animator.SetFloat("ClimbSpeed",0);
            
                if (!_isClimbingStopped)
                {
                    _isClimbingStopped = true;
                }
            }
        }
        
    }
    
    /// <summary>
    /// 壁の法線を基準に、壁に沿った右方向と上方向を計算する
    /// </summary>
    private void CalculateClimbDirections()
    {
        _climbDirectionUp = Vector3.Cross(Vector3.right, _wallNormal).normalized; // 壁に沿った縦方向
        _climbDirectionParallel = Vector3.Cross(_climbDirectionUp, _wallNormal).normalized; //壁に沿った横方向
    }
}