using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{ 
    [SerializeField] private Transform _playerTransform; // プレイヤーのTransform
    [SerializeField] private CinemachineVirtualCamera _playerCamera; // カメラ（任意のカメラ）
    
    [SerializeField] private float _runSpeed = 5f, _warkSpeed = 2f; // 移動速度
    [SerializeField] private float _jumpPower = 5f;
    [SerializeField] private StepFunction _stepFunction; 
    private Rigidbody _rb; // Rigidbodyコンポーネント
    private Vector3 _moveDirection; // 入力された方向
    private float _moveSpeed; // 移動する速度

    private bool _isWarking = true; //歩いているか
    private bool _isGround; //地面についているか（検討）
    private bool _isCrouching; //しゃがんでいるか
    private bool _canMove = true; //動けるか
    private bool _isWall; //壁に足をついているか（検討）
    private bool _isGuard; //ガード状態か
    
    public bool IsGround{set => _isGround = value; }
    public bool IsWall { set => _isWall = value; }
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();// Rigidbodyを取得
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
            _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            _isGround = false;
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
            _rb.AddForce(new Vector3(100,0,0), ForceMode.Impulse);
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

    private void FixedUpdate()
    {
        if (_moveDirection.sqrMagnitude > 0.01f)　//入力がある場合のみ処理を行う
        {
            // カメラの正面方向を取得（Y軸を無視して水平面に投影）
            Vector3 cameraForward = Vector3.ProjectOnPlane(_playerCamera.transform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
        
            // 入力方向をカメラの向きに回転
            Vector3 adjustedMoveDirection = cameraForward * _moveDirection.z + cameraRight * _moveDirection.x;
        
            _rb.velocity = adjustedMoveDirection * _moveSpeed + new Vector3(0, _rb.velocity.y, 0); // プレイヤーを動かす
        
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            _playerTransform.rotation = Quaternion.Slerp(_playerTransform.rotation, targetRotation, Time.deltaTime * 10f); // スムーズな回転
        }
        else
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);　// 入力がない場合は移動速度をゼロにする
        }
    }
}