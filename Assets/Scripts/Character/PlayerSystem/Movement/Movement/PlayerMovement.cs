using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using PlayerSystem.State;
using UnityEngine;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// プレイヤーの移動機能
    /// </summary>
    public class PlayerMovement : IMovable
    {
        private PlayerBlackBoard _bb;
        private CharacterController _characterController;
        private Animator _animator;
        private CinemachineVirtualCamera _playerCamera;
        private Vector3 _moveNormal;
        private TrailRenderer _trailRenderer;
        
        private readonly float _rotationSpeed = 10f;

        public PlayerMovement(
            PlayerBlackBoard bb, CharacterController characterController, Animator animator,
            CinemachineVirtualCamera playerCamera, TrailRenderer trailRenderer)
        {
            _bb = bb;
            _characterController = characterController;
            _animator = animator;
            _playerCamera = playerCamera;
            _trailRenderer = trailRenderer;
        }
        
        public void Move()
        {
            if (!_bb.IsAttacking)
            {
                HandleMovement();
            }
        }
        
        /// <summary>
        /// 入力に基づいて移動処理を行う
        /// </summary>
        private void HandleMovement()
        {
            if (_bb.MoveDirection.sqrMagnitude > 0.01f)　//入力がある場合のみ処理を行う
            {
                _trailRenderer.emitting = true; //軌跡をつける

                // カメラ基準で移動方向を計算
                Vector3 cameraForward = Vector3.ProjectOnPlane(_playerCamera.transform.forward, Vector3.up).normalized;
                Vector3 cameraRight = Vector3.ProjectOnPlane(_playerCamera.transform.right, Vector3.up).normalized;
                Vector3 moveDirection = cameraForward * _bb.MoveDirection.z + cameraRight * _bb.MoveDirection.x;

                _bb.CorrectedDirection = moveDirection;
                _moveNormal = moveDirection.normalized;

                // 回転をカメラの向きに合わせる
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                _characterController.transform.rotation = Quaternion.Slerp(
                    _characterController.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

                // Animatorの速度を設定
                _animator.SetFloat("Speed", _moveNormal.sqrMagnitude * _bb.MoveSpeed, 0.1f, Time.deltaTime);
            }
            else
            {
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