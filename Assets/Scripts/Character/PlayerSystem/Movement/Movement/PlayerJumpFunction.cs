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
    public class PlayerJumpFunction : IJumpable
    {
        private PlayerBlackBoard _bb;
        private CharacterController _characterController;
        private Animator _animator;
        private TrailRenderer _trailRenderer;
        
        private MovementHelper _helper;
     
        private readonly float _jumpPower = 0.7f;
        private readonly float _jumpMoveSpeed = 2f; //ジャンプ中の移動速度
        private readonly float _gravity = -17.5f;

        public PlayerJumpFunction(
            PlayerBlackBoard bb, CharacterController characterController, Animator animator,
            TrailRenderer trailRenderer, MovementHelper helper)
        {
            _bb = bb;
            _characterController = characterController;
            _animator = animator;
            _trailRenderer = trailRenderer;
            _helper = helper;
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
            if (_bb.MoveDirection.sqrMagnitude > 0.01f)　//入力がある場合
            {
                _trailRenderer.emitting = true; //軌跡をつける

                _helper.RotateCharacter(_helper.CalculateMoveDirection());

                //ルートモーションがオフなので、CharacterControllerのMoveメソッドを使用する

                float velocityY = _bb.Velocity.y; //Y軸の速度を保存する
                //移動中の速度は入力方向×ジャンプ中のスピード×現在のスピード（歩き/走り）
                _bb.Velocity = new Vector3(_bb.CorrectedDirection.x * _jumpMoveSpeed * _bb.MoveSpeed,
                    velocityY, _bb.CorrectedDirection.z * _jumpMoveSpeed * _bb.MoveSpeed);

                _characterController.Move(_bb.Velocity * Time.deltaTime);
            }
            else // 入力がない場合
            {
                float velocityY = _bb.Velocity.y; //Y軸の速度を保存する
                _bb.Velocity = new Vector3(0, velocityY, 0);

                _characterController.Move(_bb.Velocity * Time.deltaTime);
            }
        }
    }
}