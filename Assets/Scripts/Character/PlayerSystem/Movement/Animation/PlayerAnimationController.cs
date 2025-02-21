using UnityEngine;

namespace PlayerSystem.Animation
{
    /// <summary>
    /// プレイヤーのアニメーション制御を担当するクラス
    /// </summary>
    public class PlayerAnimationController
    {
        private PlayerAnimation_Common _common;
        private PlayerAnimation_Movement _movement;
        
        public PlayerAnimation_Common Common => _common;
        public PlayerAnimation_Movement Movement => _movement;

        public PlayerAnimationController(Animator animator)
        {
            _common = new PlayerAnimation_Common(animator);
            _movement = new PlayerAnimation_Movement(animator);
        }
    }
}
