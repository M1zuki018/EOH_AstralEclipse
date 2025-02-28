using PlayerSystem.State;
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
        private PlayerAnimation_Combat _combat;
        private PlayerStandbyMotionFunction _standbyMotionFunc; // 待機モーションを再生するクラス

        public PlayerAnimation_Common Common => _common;
        public PlayerAnimation_Movement Movement => _movement;
        public PlayerAnimation_Combat Combat => _combat;

        public PlayerAnimationController(PlayerBlackBoard bb, Animator animator)
        {
            _common = new PlayerAnimation_Common(animator);
            _movement = new PlayerAnimation_Movement(animator);
            _combat = new PlayerAnimation_Combat(animator);
            _standbyMotionFunc = new PlayerStandbyMotionFunction(bb, this);
        }
    }
}
