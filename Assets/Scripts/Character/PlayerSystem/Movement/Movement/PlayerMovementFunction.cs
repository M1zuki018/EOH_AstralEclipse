using PlayerSystem.State;
using UnityEngine;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// プレイヤーの移動機能
    /// </summary>
    public class PlayerMovementFunction : IMovable
    {
        private PlayerBlackBoard _bb;
        private Animator _animator;
        private Vector3 _moveNormal;
        private TrailRenderer _trailRenderer;
        
        private MovementHelper _helper;

        public PlayerMovementFunction(
            PlayerBlackBoard bb, Animator animator,TrailRenderer trailRenderer, MovementHelper helper)
        {
            _bb = bb;
            _animator = animator;
            _trailRenderer = trailRenderer;
            _helper = helper;
        }
        
        public void Move()
        {
            if (!_bb.IsAttacking)
            {
                HandleMovement();
            }
        }
        
        /// <summary>
        /// 入力に基づいて移動処理を行う
        /// </summary>
        private void HandleMovement()
        {
            if (_bb.MoveDirection.sqrMagnitude > 0.01f)　//入力がある場合のみ処理を行う
            {
                _trailRenderer.emitting = true; //軌跡をつける
                
                _helper.RotateCharacter(_helper.CalculateMoveDirection());
                    
                // Animatorの速度を設定
                _animator.SetFloat("Speed", _bb.CorrectedDirection.sqrMagnitude * _bb.MoveSpeed, 0.1f, Time.deltaTime);
            }
            else
            {
                //緩やかに減速する。2fの部分を変化させると、減速の強さを変更できる
                _bb.CorrectedDirection = Vector3.Lerp(_bb.CorrectedDirection, Vector3.zero, 2f * Time.deltaTime);
                float speed = _bb.CorrectedDirection.magnitude * _bb.MoveSpeed;
                
                if (speed < 0.03f)
                {
                    //減速がほぼ終了していたら、スピードにはゼロを入れる
                    _animator.SetFloat("Speed", 0);
                    _trailRenderer.emitting = false; //TrailRendererの描写は行わない
                }
                else
                {
                    _trailRenderer.emitting = true;
                    _animator.SetFloat("Speed", speed);   
                }
            }
        }
    }

}