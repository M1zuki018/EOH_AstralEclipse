using System;
using Cinemachine;
using PlayerSystem.State;
using UniRx;
using UnityEngine;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// プレイヤーの移動・ジャンプ・歩き/走りの切り替え処理を包括したクラス
    /// </summary>
    public class PlayerControlFunction : IMovable, IJumpable, IWalkable
    {
        private CharacterController _characterController;
        private Animator _animator;
        private PlayerBlackBoard _blackBoard;
        private CinemachineVirtualCamera _playerCamera;
        private Vector3 _moveNormal;
        private TrailRenderer _trailRenderer;
        
        private PlayerMovement _movement;
        private PlayerJump _jump;
        
        private IDisposable _walkChangedSubscription;

        private readonly float _runSpeed = 2f;
        private readonly float _walkSpeed = 1f;
        private readonly float _jumpPower = 0.7f;
        private readonly float _jumpMoveSpeed = 2f; //ジャンプ中の移動速度
        private readonly float _gravity = -17.5f;
        private readonly float _rotationSpeed = 10f;
        private readonly float _climbSpeed = 3f;
        
        public PlayerControlFunction(
            PlayerMovement movement, PlayerJump jump,
            CharacterController characterController, Animator animator, PlayerBlackBoard blackBoard, 
            CinemachineVirtualCamera playerCamera, TrailRenderer trailRenderer)
        {
            _movement = movement;
            _jump = jump;
            _characterController = characterController;
            _animator = animator;
            _blackBoard = blackBoard;
            _playerCamera = playerCamera;
            _trailRenderer = trailRenderer;
            _blackBoard.MoveSpeed = _walkSpeed;
        }

        /// <summary>
        /// 移動機能の呼び出し
        /// </summary>
        public void Move() => _movement.Move();
        
        /// <summary>
        /// ジャンプ機能の実装
        /// </summary>
        public void Jump() => _jump.Jump();
        // {
        //     if (_blackBoard.IsGrounded) //地面にいる場合はジャンプの初速度を設定する
        //     {
        //         _blackBoard.IsJumping = true;
        //         _blackBoard.IsGrounded = false; //TODO:接地判定の切り替えをここに書くべきか？
        //         _blackBoard.Velocity = new Vector3(0f, Mathf.Sqrt(_jumpPower * -2f * _gravity), 0f); //初速度を計算
        //         _animator.SetTrigger("Jump");
        //         _animator.SetBool("IsJumping", true);
        //         _animator.applyRootMotion = false;
        //     }
        // }

        /// <summary>
        /// ジャンプ中の処理の実装
        /// </summary>
        public void Jumping() => _jump.Jumping();
        // {
        //     HandleMovement();
        //     ApplyGravity();
        // }

        /// <summary>
        /// 動きの速度を切り替える実装
        /// </summary>
        public void Walk()
        {
            // 黒板のWalkingのbool値が変更されたとき、移動速度を変更する
            _walkChangedSubscription = _blackBoard.IsWalking
                .DistinctUntilChanged()
                .Subscribe(_ => _blackBoard.MoveSpeed = _blackBoard.IsWalking.Value ? _walkSpeed : _runSpeed);
        }

        /// <summary>
        /// 購読解除
        /// </summary>
        public void DisposeWalkSubscription()
        {
            _walkChangedSubscription?.Dispose();
        }

        /// <summary>
        /// 入力に基づいて移動処理を行う
        /// </summary>
        private void HandleMovement()
        {
            if (_blackBoard.MoveDirection.sqrMagnitude > 0.01f)　//入力がある場合のみ処理を行う
            {
                _trailRenderer.emitting = true; //軌跡をつける
                
                // カメラ基準で移動方向を計算
                Vector3 cameraForward = Vector3.ProjectOnPlane(_playerCamera.transform.forward, Vector3.up).normalized;
                Vector3 cameraRight = Vector3.ProjectOnPlane(_playerCamera.transform.right, Vector3.up).normalized;
                Vector3 moveDirection = cameraForward *_blackBoard.MoveDirection.z + cameraRight * _blackBoard.MoveDirection.x;
                
                _blackBoard.CorrectedDirection = moveDirection;
                _moveNormal = moveDirection.normalized;
                
                // 回転をカメラの向きに合わせる
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                _characterController.transform.rotation = Quaternion.Slerp(
                    _characterController.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

                if (_animator.applyRootMotion)
                {
                    // Animatorの速度を設定
                    _animator.SetFloat("Speed", _moveNormal.sqrMagnitude * _blackBoard.MoveSpeed, 0.1f, Time.deltaTime);
                }
                else
                {
                    //ルートモーションがオンじゃないとき＝ジャンプ中は、CharacterControllerのMoveメソッドを使用する
                    
                    float velocityY = _blackBoard.Velocity.y; //Y軸の速度を保存する
                    //移動中の速度は入力方向×ジャンプ中のスピード×現在のスピード（歩き/走り）
                    _blackBoard.Velocity = new Vector3(_moveNormal.x * _jumpMoveSpeed * _blackBoard.MoveSpeed,
                        velocityY, _moveNormal.z * _jumpMoveSpeed * _blackBoard.MoveSpeed);
                    
                    _characterController.Move(_blackBoard.Velocity * Time.deltaTime);
                }
            }
            else
            {
                //ジャンプ中の処理
                if (_blackBoard.IsJumping)
                {
                    float velocityY = _blackBoard.Velocity.y; //Y軸の速度を保存する
                    _blackBoard.Velocity = new Vector3(0, velocityY, 0);
                    
                    _characterController.Move(_blackBoard.Velocity * Time.deltaTime);
                }
                //緩やかに減速する。2fの部分を変化させると、減速の強さを変更できる
                _moveNormal = Vector3.Lerp(_moveNormal, Vector3.zero, 2f * Time.deltaTime);
                float speed = _moveNormal.magnitude * _blackBoard.MoveSpeed;
                
                if (speed < 0.03f)
                {
                    //減速がほぼ終了していたら、スピードにはゼロを入れる
                    _animator.SetFloat("Speed", 0);
                    _trailRenderer.emitting = false; //TrailRendererの描写は行わない
                }
                else
                {
                    _trailRenderer.emitting = true;
                    _animator.SetFloat("Speed", speed);   
                }
            }
        }
        
        /// <summary>
        /// 重力を適用する
        /// </summary>
        private void ApplyGravity()
        {
            if (_blackBoard.ApplyGravity)
            {
                Vector3 velocity = _blackBoard.Velocity;
                velocity.y += _gravity * Time.deltaTime;
                _blackBoard.Velocity = velocity;
                _characterController.Move(_blackBoard.Velocity * Time.deltaTime); // 垂直方向の速度を反映
            }
        }
        
    }
    
}
