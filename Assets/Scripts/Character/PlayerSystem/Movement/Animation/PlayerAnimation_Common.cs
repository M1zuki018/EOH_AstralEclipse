using UnityEngine;

namespace PlayerSystem.Animation
{
    /// <summary>
    /// プレイヤーのCommonアニメーション
    /// </summary>
    public class PlayerAnimation_Common
    {
        private Animator _animator;

        public PlayerAnimation_Common(Animator animator)
        {
            _animator = animator;
        }
        
        /// <summary>
        /// アイドルモーションの抽選・再生を行う
        /// </summary>
        public void PlayRandomIdleMotion()
        {
            int rand = Random.Range(0, 2); //モーションの抽選
            _animator.SetBool("BackToIdle", false); //falseに戻しておく
            _animator.SetInteger("IdleType", rand);
            _animator.SetTrigger("PlayIdle");
        }
        
        /// <summary>
        /// ダメージアニメーション
        /// </summary>
        public void PlayDamageAnimation()
        {
            _animator.SetTrigger("Damage");
        }

        /// <summary>
        /// 死亡アニメーション
        /// </summary>
        public void PlayDeathAnimation()
        {
            _animator.SetTrigger("IsDeath");
        }
    }
}
