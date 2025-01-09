using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{ 
    [SerializeField] private Transform _playerTransform; // プレイヤーのTransform
    [SerializeField] private CinemachineVirtualCamera _playerCamera; // カメラ（任意のカメラ）
    [SerializeField] private Animator _animator;
    private CharacterController _characterController;
    
    [Header("キャラクター設定")]
    [SerializeField] private float _runSpeed = 5f, _warkSpeed = 2f; // 移動速度
    [SerializeField] private float _jumpPower = 5f; //ジャンプの高さ
    [SerializeField] private float _gravity = -9.81f; //重力
    [SerializeField] private float _rotationSpeed = 10f; //回転速度
    
    [Header("各種機能")]
    [SerializeField] private StepFunction _stepFunction; 
    [SerializeField] private LockOnFunction _lockOnFunction; 
    
    private Vector3 _moveDirection; // 入力された方向
    private Vector3 _velocity; //垂直方向の速度
    private float _moveSpeed; // 移動する速度
    

    private bool _isWarking = true; //歩いているか
    private bool _isGround; //地面についているか
    private bool _isCrouching; //しゃがんでいるか
    private bool _isJumping;
    private bool _canMove = true; //動けるか
    private bool _isWall; //壁に足をついているか
    private bool _isGuard; //ガード状態か
    
    public bool IsGround{set => _isGround = value; }
    public bool IsWall { set => _isWall = value; }
    
    /// <summary>ヴォルトできるか</summary>
    public bool IsVault { get; set; }
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        
        _animator.applyRootMotion = false; //ルートモーションを有効化
        _moveSpeed = _warkSpeed; //デフォルトは歩き状態
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed && !context.canceled)
        {
            return;
        }
        
        Vector2 inputVector = context.ReadValue<Vector2>();
        _moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
    }

    /// <summary>
    /// 歩きと走り状態を切り替える
    /// </summary>
    public void OnWark(InputAction.CallbackContext context)
    {
        //ボタンが押されたとき
        if (context.performed)
        {
            _isWarking = !_isWarking;
            _moveSpeed = _isWarking ? _warkSpeed : _runSpeed;
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
            //_rb.AddForce(new Vector3(100,0,0), ForceMode.Impulse);
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
        if (context.performed && IsVault)
        {
            Debug.Log("Vault 発動");
        }
    }
    
    private void FixedUpdate()
    {
        HandleGroundedCheck();
        HandleMovement();
        ApplyGravity();
        HandleJump();
    }

    /// <summary>
    /// 入力に元っづいて移動処理を行う
    /// </summary>
    private void HandleMovement()
    {
        if (_moveDirection.sqrMagnitude > 0.01f)　//入力がある場合のみ処理を行う
        {
            // カメラ基準で移動方向を計算
            Vector3 cameraForward = Vector3.ProjectOnPlane(_playerCamera.transform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(_playerCamera.transform.right, Vector3.up).normalized;
            Vector3 moveDirection = cameraForward * _moveDirection.z + cameraRight * _moveDirection.x;
            
            // CharacterControllerで移動
            _characterController.Move(moveDirection * _moveSpeed * Time.deltaTime);

            // 回転をカメラの向きに合わせる
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            
            // Animatorの速度を設定
            _animator.SetFloat("Speed", moveDirection.sqrMagnitude * _moveSpeed, 0.1f, Time.deltaTime);
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
}