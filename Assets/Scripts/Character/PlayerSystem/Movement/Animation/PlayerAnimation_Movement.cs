using UnityEngine;

namespace PlayerSystem.Animation
{
    /// <summary>
    /// プレイヤーの移動に関するアニメーションを管理する
    /// </summary>
    public class PlayerAnimation_Movement
    {
        private Animator _animator;

        public PlayerAnimation_Movement(Animator animator)
        {
            _animator = animator;
        }
        
        /// <summary>移動速度をアニメーションに反映</summary>
        public void SetMoveSpeed(float speed) => _animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

        /// <summary>
        /// ジャンプアニメーションを開始
        /// </summary>
        public void PlayJumpAnimation()
        {
            _animator.SetTrigger("Jump");
            _animator.SetBool("IsJumping", true);
            _animator.applyRootMotion = false;
        }

        /// <summary>
        /// ジャンプ終了時のアニメーション処理
        /// </summary>
        public void StopJumpAnimation()
        {
            _animator.SetBool("IsJumping", false);
            _animator.applyRootMotion = true;
        }

        /// <summary>ステップアニメーションを再生</summary>
        public void PlayStepAnimation() => _animator.SetTrigger("Step");

        /// <summary>ガードアニメーションを再生</summary>
        public void PlayGuardAnimation() => _animator.SetBool("Guard", true);

        /// <summary>ガードアニメーションを停止</summary>
        public void StopGuardAnimation() => _animator.SetBool("Guard", false);

        /// <summary>ガードブレイクアニメーションを再生</summary>
        public void PlayGuardBreakAnimation() => _animator.SetBool("GuardBreak", true);

        /// <summary>ガードブレイクアニメーションを停止</summary>
        public void StopGuardBreakAnimation() => _animator.SetBool("GuardBreak", false);
        
        /// <summary>接地判定がないときに落下モーションを再生する</summary>
        public void PlayFallingAnimation(bool isGrounded) => _animator.SetBool("IsGround", isGrounded);
    }
}
