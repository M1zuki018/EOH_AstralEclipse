using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{ 
    [SerializeField] private float _runSpeed = 5f, _warkSpeed = 2f; // 移動速度
    [SerializeField] private float _jumpPower = 5f;
    private Rigidbody _rb;                         // Rigidbodyコンポーネント
    private Vector3 _moveDirection;               // 入力された方向
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
        _moveSpeed = _warkSpeed; //defaultは歩き状態
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    public void OnMove(InputValue input) // Input Systemの値を取得（x: 横, z: 縦）
    {
        Vector2 inputVector = input.Get<Vector2>();
        _moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
    }

    /// <summary>
    /// 歩きと走り状態を切り替える
    /// </summary>
    public void OnWark(InputValue input)
    {
        _isWarking = !_isWarking;
        _moveSpeed = _isWarking ? _warkSpeed : _runSpeed;
    }

    /// <summary>
    /// しゃがみ状態を切り替える
    /// </summary>
    public void OnCrouch(InputValue input)
    {
        _isCrouching = !_isCrouching;
        ///TODO; 処理を書く
    }

    private void FixedUpdate()
    {
        //Debug.Log(_isCrouching);
        _rb.velocity = _moveDirection * _moveSpeed + new Vector3(0, _rb.velocity.y, 0); // プレイヤーを動かす
    }
}