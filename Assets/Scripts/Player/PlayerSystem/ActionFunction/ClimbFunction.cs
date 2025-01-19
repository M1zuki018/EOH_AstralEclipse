using UnityEngine;

namespace PlayerSystem.ActionFunction
{
/// <summary>
/// 壁のぼりの機能
/// </summary>
public class ClimbFunction : IClimbale
{
    private readonly Animator _animator;
    private readonly CharacterController _characterController;
    private readonly Transform _playerTransform;
    private readonly float _climbSpeed = 2f; //壁をのぼる速さ
    private readonly PlayerMovement _playerMovement;
    
    private Vector3 _wallNormal;
    private bool _isClimbingStopped;
    private Vector3 _climbDirectionUp;
    private Vector3 _climbDirectionParallel;

    public bool IsClimbing { get; }
    
    
    public ClimbFunction(Animator animator, CharacterController characterController, Transform playerTransform, 
        float climbSpeed, PlayerMovement playerMovement)
    {
        _animator = animator;
        _characterController = characterController;
        _playerTransform = playerTransform;
        _climbSpeed = climbSpeed;
        _playerMovement = playerMovement;
    }

    /// <summary>
    /// 壁のぼりはじめの処理
    /// </summary>
    public void StartClimbing()
    {
        SetWallNormal();
        _playerMovement.PlayerState.IsClimbing = true;
        _animator.SetTrigger("Climb");
        _animator.SetBool("IsClimbing", true);
        _animator.applyRootMotion = false; //ルートモーションを適用しない
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
    /// 壁のぼり中の移動処理
    /// </summary>
    public void HandleClimbing(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 climbDirection = Quaternion.LookRotation(-_wallNormal) * moveDirection;
            _characterController.Move(climbDirection * _climbSpeed * Time.deltaTime);
            _animator.SetFloat("ClimbSpeed", climbDirection.magnitude, 0.1f, Time.deltaTime);
        }
        else
        {
            StopClimbingAnimation();
        }
    }

    /// <summary>
    /// 壁の法線を基準に、壁に沿った右方向と上方向を計算する
    /// </summary>
    public void SetWallNormal()
    {
        _climbDirectionUp = Vector3.Cross(Vector3.right, _playerMovement.PlayerState.WallNormal).normalized; // 壁に沿った縦方向
        _climbDirectionParallel = Vector3.Cross(_climbDirectionUp, _playerMovement.PlayerState.WallNormal).normalized; //壁に沿った横方向
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
    
}
