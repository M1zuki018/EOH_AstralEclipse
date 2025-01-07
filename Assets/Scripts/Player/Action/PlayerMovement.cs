using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{ 
    [SerializeField] private float _runSpeed = 5f, _warkSpeed = 2f; // 移動速度
    [SerializeField] private float _jumpPower = 5f;
    private Rigidbody _rb; // Rigidbodyコンポーネント
    private Vector3 _moveDirection; // 入力された方向
    private float _moveSpeed; // 移動する速度

    private bool _isWarking = true; //歩いているか
    private bool _isGround; //地面についているか（検討）
    private bool _isCrouching; //しゃがんでいるか
    private bool _canMove = true; //動けるか
    private bool _isWall; //壁に足をついているか（検討）
    
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
            Debug.Log("しゃがみ中");
        }
        
        //ボタンが放されたとき
        if (context.canceled)
        {
            _isCrouching = false;
            Debug.Log("しゃがみ状態解除");
        }
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        // 入力されたとき地面にいるときのみジャンプ可能
        if (context.performed && _isGround)
        {
            _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            _isGround = false;
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log(_moveSpeed);
        _rb.velocity = _moveDirection * _moveSpeed + new Vector3(0, _rb.velocity.y, 0); // プレイヤーを動かす
    }
}