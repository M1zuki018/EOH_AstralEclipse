using PlayerSystem.Animation;
using PlayerSystem.State;
using UnityEngine;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// プレイヤーのジャンプ機能
    /// </summary>
    public class PlayerJumpFunction : IJumpable
    {
        private readonly PlayerBlackBoard _bb;
        private readonly CharacterController _cc;
        private readonly PlayerAnimationController _animController;
        private readonly MovementHelper _helper;

        public PlayerJumpFunction(
            PlayerBlackBoard bb, CharacterController cc, PlayerAnimationController animController, MovementHelper helper)
        {
            _bb = bb;
            _cc = cc;
            _animController = animController;
            _helper = helper;
        }
        
        /// <summary>
        /// ジャンプ機能
        /// </summary>
        public void Jump()
        {
            _bb.IsJumping = true;
            _bb.IsGrounded = false; //TODO:接地判定の切り替えをここに書くべきか？
            _bb.Velocity = new Vector3(0f, Mathf.Sqrt(_bb.Data.JumpPower * -2f * _bb.Data.Gravity), 0f); //初速度を計算
            _animController.Movement.PlayJumpAnimation();
        }

        /// <summary>
        /// ジャンプ中の処理
        /// </summary>
        public void Jumping()
        {
            if (_bb.MoveDirection.sqrMagnitude > 0.01f)　//入力がある場合
            {
                _helper.RotateCharacter(_helper.CalculateMoveDirection());

                //ルートモーションがオフなので、CharacterControllerのMoveメソッドを使用する

                float velocityY = _bb.Velocity.y; //Y軸の速度を保存する
                //移動中の速度は入力方向×ジャンプ中のスピード×現在のスピード（歩き/走り）
                _bb.Velocity = new Vector3(_bb.CorrectedDirection.x * _bb.Data.JumpMoveSpeed * _bb.MoveSpeed,
                    velocityY, _bb.CorrectedDirection.z * _bb.Data.JumpMoveSpeed * _bb.MoveSpeed);

                _cc.Move(_bb.Velocity * Time.deltaTime);
            }
            else // 入力がない場合
            {
                float velocityY = _bb.Velocity.y; //Y軸の速度を保存する
                _bb.Velocity = new Vector3(0, velocityY, 0);

                _cc.Move(_bb.Velocity * Time.deltaTime);
            }
        }
    }
}