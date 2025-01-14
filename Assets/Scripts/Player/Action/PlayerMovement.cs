using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{ 
    [SerializeField] private Transform _playerTransform; // プレイヤーのTransform
    [SerializeField] private CinemachineVirtualCamera _playerCamera; // カメラ（任意のカメラ）
    public Animator _animator;
    private CharacterController _characterController;
    
    [Header("キャラクター設定")]
    [SerializeField] private float _runSpeed = 5f, _warkSpeed = 2f; // 移動速度
    [SerializeField] private float _jumpPower = 5f; //ジャンプの高さ
    [SerializeField] private float _gravity = -9.81f; //重力
    [SerializeField] private float _rotationSpeed = 10f; //回転速度
    [SerializeField] private float _climbSpeed = 3f; //壁を登る速さ
    
    [Header("各種機能")]
    [SerializeField] private StepFunction _stepFunction; 
    [SerializeField] private LockOnFunction _lockOnFunction; 
    [SerializeField] private WallRunFunction _wallRunFunction;
    [SerializeField] private VaultFunction _vaultFunction;
    [SerializeField] private BigJumpFunction _bigJumpFunction;
    
    private Vector3 _moveDirection; // 入力された方向
    private Vector3 _velocity; //垂直方向の速度
    private float _moveSpeed; // 移動する速度
    

    private bool _isWarking = true; //歩いているか
    private bool _isGround; //地面についているか
    private bool _isCrouching; //しゃがんでいるか
    private bool _isJumping; //ジャンプ中か
    
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
    
    private bool _canMove = true; //動けるか
    private bool _isWall; //壁にいるか
    private bool _isGuard; //ガード状態か
    
    public bool IsGround
    {
        get { return _isGround;}
        set => _isGround = value; }
    public bool IsWall { set => _isWall = value; }
    
    public bool CanClimb { set => _canClimb = value; }
    public Vector3 WallNormal { set => _wallNormal = value; }
    
    /// <summary>ヴォルトできるか</summary>
    public bool CanVault { get; set; }
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        
        _animator.applyRootMotion = true; //ルートモーションを有効化
        _moveSpeed = _warkSpeed; //デフォルトは歩き状態
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _animator.SetTrigger("Idle");
    }
    
    /// <summary>
    /// 移動処理
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed && !context.canceled)
            return;
        
        Vector2 inputVector = context.ReadValue<Vector2>();
        
        if (_isClimbing) //壁のぼり中
        {
            _moveDirection = new Vector3(0, inputVector.y, -inputVector.x);
        }
        else
        {
            _moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
        }
    }

    /// <summary>
    /// 歩きと走り状態を切り替える
    /// </summary>
    public void OnWark(InputAction.CallbackContext context)
    {
        //ボタンが押されたとき
        if (context.performed)
        {
            if (!_isClimbing) //壁のぼり中以外
            {
                _isWarking = !_isWarking;
                _moveSpeed = _isWarking ? _warkSpeed : _runSpeed;
            }
            else
            {
                _moveSpeed = _climbSpeed; //壁を登っている時は、moveSpeedに壁のぼりの速度を代入する
            }
        }
    }

    /// <summary>
    /// しゃがみ状態を切り替える
    /// </summary>
    public void OnCrouch(InputAction.CallbackContext context)
    {
        //ボタンが押されたとき
        if (context.performed)
        {
            _isCrouching = true;
        }
        
        //ボタンが放されたとき
        if (context.canceled)
        {
            _isCrouching = false;
        }
    }
    
    /// <summary>
    /// ジャンプ
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        // 入力されたとき地面にいるときのみジャンプ可能
        if (context.performed && _isGround)
        {
            _isJumping = true;
            _isGround = false;
            _velocity.y = Mathf.Sqrt(_jumpPower * -2f * _gravity); // 初速度を計算
            _animator.SetTrigger("Jump"); // ジャンプアニメーションをトリガー
            _animator.SetBool("IsJumping", true);
            _animator.applyRootMotion = false;
        }
    }

    /// <summary>
    /// ステップ
    /// </summary>
    public void OnStep(InputAction.CallbackContext context)
    {
        if (context.performed && _stepFunction.TryUseStep())
        {
            _stepFunction.TryUseStep();
        }
    }
    
    /// <summary>
    /// ガード状態を切り替える
    /// </summary>
    public void OnGuard(InputAction.CallbackContext context)
    {
        //ボタンが押されたとき
        if (context.performed)
        {
            _isGuard = true;
        }
        
        //ボタンが放されたとき
        if (context.canceled)
        {
            _isGuard = false;
        }
    }

    /// <summary>
    /// ロックオン機能
    /// </summary>
    public void OnLockOn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _lockOnFunction.LockOnToEnemy();
        }
    }

    /// <summary>
    /// パルクールアクションキー
    /// </summary>
    public void OnParkourAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanVault) 
            {
                _vaultFunction.HandleVault(this);//ヴォルトアクション
            }
            else if (_canClimb)
            {
                //壁のぼりアクション
                _isClimbing = !_isClimbing;
                
                if (_isClimbing) //壁のぼり開始なら
                {
                    _isJumping = false;
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
                    _isGround = false;
                }
            }
            else if (_bigJumpFunction.CanJump)
            {
                _bigJumpFunction.HandleBigJump(this);
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (_isClimbing) //壁のぼり中
        {
            HandleClimbing();
        }
        else
        {
            HandleGroundedCheck();
            HandleMovement();
            ApplyGravity();
            HandleJump();
            HandleFalling();
        }
    }

    /// <summary>
    /// 入力に基づいて移動処理を行う
    /// </summary>
    private void HandleMovement()
    {
        if (_moveDirection.sqrMagnitude > 0.01f)　//入力がある場合のみ処理を行う
        {
            
            // カメラ基準で移動方向を計算
            Vector3 cameraForward = Vector3.ProjectOnPlane(_playerCamera.transform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(_playerCamera.transform.right, Vector3.up).normalized;
            Vector3 moveDirection = cameraForward * _moveDirection.z + cameraRight * _moveDirection.x;
            
            // 回転をカメラの向きに合わせる
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            if (_animator.applyRootMotion)
            {
                // Animatorの速度を設定
                _animator.SetFloat("Speed", moveDirection.sqrMagnitude * _moveSpeed, 0.1f, Time.deltaTime);
            }
            else
            {
                //ルートモーションがオンじゃなければ、CharacterControllerのMoveメソッドを使用する
                _characterController.Move(moveDirection * _moveSpeed * Time.deltaTime);
            }
            
        }
        else
        {
            _animator.SetFloat("Speed", 0);　// 入力がない場合は停止
        }
    }

    /// <summary>
    /// 地面にいるときの処理
    /// </summary>
    private void HandleGroundedCheck()
    {
        if (_isGround && _velocity.y < 0)
        {
            _isJumping = false;
            _velocity.y = 0f; //地面にいる場合、垂直速度をリセットする
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
        if (!_isJumping && !_isClimbing && !IsVault)
        {
            if (_isGround)
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
    /// 重力を適用する
    /// </summary>
    private void ApplyGravity()
    {
        if (!_isGround)
        {
            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime); // 垂直方向の速度を反映
        }
    }
    
    /// <summary>
    /// ジャンプ処理
    /// </summary>
    private void HandleJump()
    {
        if (_isJumping)
        {
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// 壁のぼり処理
    /// </summary>
    private void HandleClimbing()
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
    
    /// <summary>
    /// 壁の法線を基準に、壁に沿った右方向と上方向を計算する
    /// </summary>
    private void CalculateClimbDirections()
    {
        _climbDirectionUp = Vector3.Cross(Vector3.right, _wallNormal).normalized; // 壁に沿った縦方向
        _climbDirectionParallel = Vector3.Cross(_climbDirectionUp, _wallNormal).normalized; //壁に沿った横方向
    }
}