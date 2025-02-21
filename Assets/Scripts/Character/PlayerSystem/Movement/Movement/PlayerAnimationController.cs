using UnityEngine;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// プレイヤーのアニメーション制御を担当するクラス
    /// </summary>
    public class PlayerAnimationController
    {
        private Animator _animator;

        public PlayerAnimationController(Animator animator)
        {
            _animator = animator;
        }
    }

}
