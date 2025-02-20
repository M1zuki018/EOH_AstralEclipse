using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using PlayerSystem.State;
using UnityEngine;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// プレイヤーのジャンプ機能
    /// </summary>
    public class PlayerJump : IJumpable
    {
        private PlayerBlackBoard _bb;
        private CharacterController _characterController;
        private Animator _animator;
        private CinemachineVirtualCamera _playerCamera;
        private Vector3 _moveNormal;
        private TrailRenderer _trailRenderer;
        
        private PlayerGravity _playerGravity;
     
        private readonly float _jumpPower = 0.7f;
        private readonly float _jumpMoveSpeed = 2f; //ジャンプ中の移動速度
        private readonly float _rotationSpeed = 10f;
        private readonly float _gravity = -17.5f;

        public PlayerJump(
            PlayerBlackBoard bb, CharacterController characterController, Animator animator,
            CinemachineVirtualCamera playerCamera, TrailRenderer trailRenderer)
        {
            _bb = bb;
            _characterController = characterController;
            _animator = animator;
            _playerCamera = playerCamera;
            _trailRenderer = trailRenderer;
            
            _playerGravity = new PlayerGravity(_bb, _characterController);
        }
        
        /// <summary>
        /// ジャンプ機能
        /// </summary>
        public void Jump()
        {
            _bb.IsJumping = true;
            _bb.IsGrounded = false; //TODO:接地判定の切り替えをここに書くべきか？
            _bb.Velocity = new Vector3(0f, Mathf.Sqrt(_jumpPower * -2f * _gravity), 0f); //初速度を計算
            _animator.SetTrigger("Jump");
            _animator.SetBool("IsJumping", true);
            _animator.applyRootMotion = false;
        }

        /// <summary>
        /// ジャンプ中の処理
        /// </summary>
        public void Jumping()
        {
            _playerGravity.ApplyGravity();
            
            if (_bb.MoveDirection.sqrMagnitude > 0.01f)　//入力がある場合のみ処理を行う
            {
                _trailRenderer.emitting = true; //軌跡をつける
                
                // カメラ基準で移動方向を計算
                Vector3 cameraForward = Vector3.ProjectOnPlane(_playerCamera.transform.forward, Vector3.up).normalized;
                Vector3 cameraRight = Vector3.ProjectOnPlane(_playerCamera.transform.right, Vector3.up).normalized;
                Vector3 moveDirection = cameraForward *_bb.MoveDirection.z + cameraRight * _bb.MoveDirection.x;
                
                _bb.CorrectedDirection = moveDirection;
                _moveNormal = moveDirection.normalized;
                
                // 回転をカメラの向きに合わせる
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                _characterController.transform.rotation = Quaternion.Slerp(
                    _characterController.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

                if (_animator.applyRootMotion)
                {
                    // Animatorの速度を設定
                    _animator.SetFloat("Speed", _moveNormal.sqrMagnitude * _bb.MoveSpeed, 0.1f, Time.deltaTime);
                }
                else
                {
                    //ルートモーションがオンじゃないとき＝ジャンプ中は、CharacterControllerのMoveメソッドを使用する
                    
                    float velocityY = _bb.Velocity.y; //Y軸の速度を保存する
                    //移動中の速度は入力方向×ジャンプ中のスピード×現在のスピード（歩き/走り）
                    _bb.Velocity = new Vector3(_moveNormal.x * _jumpMoveSpeed * _bb.MoveSpeed,
                        velocityY, _moveNormal.z * _jumpMoveSpeed * _bb.MoveSpeed);
                    
                    _characterController.Move(_bb.Velocity * Time.deltaTime);
                }
            }
            else
            {
                //ジャンプ中の処理
                if (_bb.IsJumping)
                {
                    float velocityY = _bb.Velocity.y; //Y軸の速度を保存する
                    _bb.Velocity = new Vector3(0, velocityY, 0);
                    
                    _characterController.Move(_bb.Velocity * Time.deltaTime);
                }
                //緩やかに減速する。2fの部分を変化させると、減速の強さを変更できる
                _moveNormal = Vector3.Lerp(_moveNormal, Vector3.zero, 2f * Time.deltaTime);
                float speed = _moveNormal.magnitude * _bb.MoveSpeed;
                
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
    }
}