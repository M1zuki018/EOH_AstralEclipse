using UnityEngine;

namespace PlayerSystem.Animation
{
    /// <summary>
    /// プレイヤーのアニメーション制御を担当するクラス
    /// </summary>
    public class PlayerAnimationController
    {
        private PlayerAnimation_Movement _movement;
        
        public PlayerAnimation_Movement Movement => _movement;

        public PlayerAnimationController(Animator animator)
        {
            _movement = new PlayerAnimation_Movement(animator);
        }
        
        
    }

}
