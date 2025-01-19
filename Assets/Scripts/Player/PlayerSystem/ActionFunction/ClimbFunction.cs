using PlayerSystem.ActionFunction;
using UnityEngine;

/// <summary>
/// 壁のぼりの機能
/// </summary>
public class ClimbFunction : IClimbale
{
    private readonly Animator _animator;
    private readonly CharacterController _characterController;
    private Transform _playerTransform;
    private readonly PlayerMovement _playerMovement;

    private Vector3 _wallNormal;
    private bool _isClimbingStopped;
    private Vector3 _climbDirectionUp;
    private Vector3 _climbDirectionParallel;

    public bool IsClimbing { get; private set; }
    
    public ClimbFunction(Animator animator, CharacterController characterController, Transform playerTransform, PlayerMovement playerMovement)
    {
        _animator = animator;
        _characterController = characterController;
        _playerTransform = playerTransform;
        _playerMovement = playerMovement;
    }

    /// <summary>
    /// 壁のぼりはじめの処理
    /// </summary>
    public void StartClimbing()
    {
        SetWallNormal(); //壁の法線を取得
        _playerMovement.PlayerState.IsJumping = false; //ジャンプの途中で壁を掴んだ時、ジャンプフラグをオフにする
        _playerMovement.PlayerState.IsClimbing = true;
        _animator.SetTrigger("Climb");
        _animator.SetBool("IsClimbing", true);
        _animator.applyRootMotion = false; //ルートモーションを適用しない
    }

    /// <summary>
    /// 壁のぼり中の移動処理
    /// </summary>
    public void HandleClimbing()
    {
        if (_playerMovement.PlayerState.MoveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 direction = _playerMovement.PlayerState.MoveDirection;
            _playerTransform.rotation = Quaternion.LookRotation(_climbDirectionParallel, Vector3.up);
            _characterController.Move(direction * _playerMovement.PlayerState.MoveSpeed * Time.deltaTime);
            _animator.SetFloat("ClimbSpeed", direction.magnitude, 0.1f, Time.deltaTime);
            
        }
        else
        {
            StopClimbingAnimation();
        }
    }
    
    /// <summary>
    /// 壁のぼり終了時の処理
    /// </summary>
    public void EndClimbing()
    {
        _playerMovement.PlayerState.IsClimbing = false;
        _animator.SetBool("IsClimbing", false);
        _isClimbingStopped = false; //停止フラグをリセット
    }

    /// <summary>
    /// 壁の法線を基準に、壁に沿った右方向と上方向を計算する
    /// </summary>
    private void SetWallNormal()
    {
        _wallNormal = _playerMovement.PlayerState.WallNormal;   
        _climbDirectionUp = Vector3.Cross(Vector3.right, _wallNormal).normalized; // 壁に沿った縦方向
        _climbDirectionParallel = Vector3.Cross(_climbDirectionUp, _wallNormal).normalized; //壁に沿った横方向
    }

    /// <summary>
    /// 停止時の処理
    /// </summary>
    private void StopClimbingAnimation()
    {
        _animator.SetFloat("ClimbSpeed", 0);

        if (!_isClimbingStopped)
        {
            _isClimbingStopped = true;
        }
    }
}
