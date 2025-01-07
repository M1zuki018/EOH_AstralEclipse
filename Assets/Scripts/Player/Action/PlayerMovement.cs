using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{ 
    [SerializeField] private float _runSpeed = 5f, _warkSpeed = 2f; // 移動速度
    private Rigidbody _rb;                         // Rigidbodyコンポーネント
    private Vector3 _moveDirection;               // 入力された方向
    private float _moveSpeed; // 移動する速度

    private bool _isWarking = true;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();// Rigidbodyを取得
        _moveSpeed = _warkSpeed; //defaultは歩き状態
    }

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

    private void FixedUpdate()
    {
        _rb.velocity = _moveDirection * _moveSpeed + new Vector3(0, _rb.velocity.y, 0); // プレイヤーを動かす
    }
}