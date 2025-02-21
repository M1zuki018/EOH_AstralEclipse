using PlayerSystem.State;
using UnityEngine;

namespace PlayerSystem.Animation
{
    /// <summary>
    /// プレイヤーのアニメーション制御を担当するクラス
    /// </summary>
    public class PlayerAnimationController
    {
        private PlayerBlackBoard _bb;
        
        private PlayerAnimation_Common _common;
        private PlayerAnimation_Movement _movement;
        private PlayerStandbyMotionFunction _standbyMotionFunc; // 待機モーションを再生するクラス

        public PlayerBlackBoard BB => _bb;
        public PlayerAnimation_Common Common => _common;
        public PlayerAnimation_Movement Movement => _movement;

        public PlayerAnimationController(PlayerBlackBoard bb, Animator animator)
        {
            _bb = bb;
            _common = new PlayerAnimation_Common(animator);
            _movement = new PlayerAnimation_Movement(animator);
            _standbyMotionFunc = new PlayerStandbyMotionFunction(_bb, this);
        }
    }
}
