using PlayerSystem.Animation;
using PlayerSystem.State;
using UnityEngine;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// プレイヤーの移動機能
    /// </summary>
    public class PlayerMovementFunction : IMovable
    {
        private readonly PlayerBlackBoard _bb;
        private readonly PlayerAnimationController _animController;
        private readonly MovementHelper _helper;

        public PlayerMovementFunction(PlayerBlackBoard bb, PlayerAnimationController animController, MovementHelper helper)
        {
            _bb = bb;
            _animController = animController;
            _helper = helper;
        }
        
        public void Move()
        {
            // 攻撃中/ガード中/ガードブレイク中は動けない
            if (!_bb.IsAttacking && !_bb.IsGuarding && !_bb.IsGuardBreak)
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
                //_trailController.EnableTrail(); //軌跡をつける
                
                _helper.RotateCharacter(_helper.CalculateMoveDirection());
                    
                // Animatorの速度を設定
                _animController.Movement.SetMoveSpeed(_bb.CorrectedDirection.sqrMagnitude * _bb.MoveSpeed);
            }
            else
            {
                //緩やかに減速する。2fの部分を変化させると、減速の強さを変更できる
                _bb.CorrectedDirection = Vector3.Lerp(_bb.CorrectedDirection, Vector3.zero, 2f * Time.deltaTime);
                float speed = _bb.CorrectedDirection.magnitude * _bb.MoveSpeed;
                
                if (speed < 0.03f)
                {
                    //減速がほぼ終了していたら、スピードにはゼロを入れる
                    _animController.Movement.SetMoveSpeed(0);
                    //_trailController.DisableTrail(); //TrailRendererの描写は行わない
                }
                else
                {
                    //_trailController.EnableTrail();
                    _animController.Movement.SetMoveSpeed(speed);   
                }
            }
        }
    }

}