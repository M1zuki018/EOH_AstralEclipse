using PlayerSystem.Animation;
using PlayerSystem.State;
using UnityEngine;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// 地面にいるときの処理の実装
    /// </summary>
    public class HandleGrounded : IHandleGroundedCheck
    {
        private PlayerBlackBoard _bb;
        private PlayerAnimationController _animController;

        public HandleGrounded(PlayerBlackBoard bb, PlayerAnimationController animController)
        {
            _bb = bb;
            _animController = animController;
        }
    
        /// <summary>
        /// 地面にいるときの処理
        /// </summary>
        public void HandleGroundedCheck()
        {
            if (_bb.IsGrounded && _bb.Velocity.y < 0)
            {
                _bb.IsJumping = false;
                _bb.Velocity = new Vector3(0, -0.1f, 0); //確実に地面につくように少し下向きの力を加える
                _animController.Movement.StopJumpAnimation();
            }
        }
    }
}
