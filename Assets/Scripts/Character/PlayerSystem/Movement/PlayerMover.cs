using Cinemachine;
using PlayerSystem.State;
using UnityEngine;

namespace PlayerSystem.Movement
{
    public class PlayerMover : IMovable, IJumpable, IWalkable, ICrouchable
    {
        private CharacterController _characterController;
        private Animator _animator;
        private PlayerState _state;
        private CinemachineVirtualCamera _playerCamera;

        private readonly float _runSpeed = 2f;
        private readonly float _walkSpeed = 1f;
        private readonly float _jumpPower = 5f;
        private readonly float _gravity = -9.81f;
        private readonly float _rotationSpeed = 10f;
        private readonly float _climbSpeed = 3f;
        
        public PlayerMover(CharacterController characterController, Animator animator, PlayerState state, CinemachineVirtualCamera playerCamera)
        {
            _characterController = characterController;
            _animator = animator;
            _state = state;
            _playerCamera = playerCamera;
            _state.MoveSpeed = _walkSpeed;
        }

        /// <summary>
        /// 移動機能の実装
        /// </summary>
        public void Move()
        {
            HandleMovement();
            ApplyGravity();
        }
        
        /// <summary>
        /// ジャンプ機能の実装
        /// </summary>
        public void Jump()
        {
            if (_state.IsGrounded)
            {
                _state.IsJumping = true;
                _state.IsGrounded = false; //TODO:接地判定の切り替えをここに書くべきか？
                Vector3 velocity = _state.Velocity;
                velocity.y += Mathf.Sqrt(_jumpPower * -2f * _gravity); //初速度を計算
                _state.Velocity = velocity;
                _animator.SetTrigger("Jump");
                _animator.SetBool("IsJumping", true);
                _animator.applyRootMotion = false;
            }
                
        }

        /// <summary>
        /// ジャンプ中の処理の実装
        /// </summary>
        public void Jumping()
        {
            _characterController.Move(_state.Velocity * Time.deltaTime);
        }

        /// <summary>
        /// 動きの速度を切り替える実装
        /// </summary>
        public void Walk()
        {
            if (!_state.IsClimbing) //壁のぼり中以外
            {
                _state.IsWalking = !_state.IsWalking;
                _state.MoveSpeed = _state.IsWalking ? _walkSpeed : _runSpeed;
            }
            else
            {
                _state.MoveSpeed = _climbSpeed; //壁を登っている時は、moveSpeedに壁のぼりの速度を代入する
            }
        }

        /// <summary>
        /// しゃがみ機能の実装
        /// </summary>
        public void Crouch(bool input)
        {
            if (input) //ボタンが押されたとき
            {
                _state.IsCrouching = true;
            }
            else //ボタンが離されたとき
            {
                _state.IsCrouching = false;
            }
        }
        
        /// <summary>
        /// 入力に基づいて移動処理を行う
        /// </summary>
        private void HandleMovement()
        {
            if (_state.MoveDirection.sqrMagnitude > 0.01f)　//入力がある場合のみ処理を行う
            {
            
                // カメラ基準で移動方向を計算
                Vector3 cameraForward = Vector3.ProjectOnPlane(_playerCamera.transform.forward, Vector3.up).normalized;
                Vector3 cameraRight = Vector3.ProjectOnPlane(_playerCamera.transform.right, Vector3.up).normalized;
                Vector3 moveDirection = cameraForward *_state.MoveDirection.z + cameraRight * _state.MoveDirection.x;
                Vector3 moveNormal = moveDirection.normalized;
            
                // 回転をカメラの向きに合わせる
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                _characterController.transform.rotation = Quaternion.Slerp(_characterController.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

                if (_animator.applyRootMotion)
                {
                    // Animatorの速度を設定
                    _animator.SetFloat("Speed", moveNormal.sqrMagnitude * _state.MoveSpeed, 0.1f, Time.deltaTime);
                }
                else
                {
                    //ルートモーションがオンじゃなければ、CharacterControllerのMoveメソッドを使用する
                    _characterController.Move(moveDirection * _state.MoveSpeed * Time.deltaTime);
                }
            
            }
            else
            {
                _animator.SetFloat("Speed", 0);　// 入力がない場合は停止
            }
        }
        
        /// <summary>
        /// 重力を適用する
        /// </summary>
        private void ApplyGravity()
        {
            if (!_state.IsGrounded)
            {
                Vector3 velocity = _state.Velocity;
                velocity.y += _gravity * Time.deltaTime;
                _state.Velocity = velocity;
                _characterController.Move(_state.Velocity * Time.deltaTime); // 垂直方向の速度を反映
            }
        }
        
    }
    
}
