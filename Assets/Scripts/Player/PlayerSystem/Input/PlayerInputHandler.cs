using UnityEngine;
using UnityEngine.InputSystem;
using PlayerSystem.Movement;
using PlayerSystem.State;

namespace PlayerSystem.Input
{
    public class PlayerInputHandler : IInputHandler
    {
        private PlayerState _state;
        private PlayerMover _playerMover;
        private IMovable _mover;
        private IJumpable _jumper;

        public PlayerInputHandler(PlayerState state, IMovable mover, IJumpable jumper)
        {
            _state = state;
            _mover = mover;
            _jumper = jumper;
        }
        
        /// 以降、動作はインターフェースを介して行う
        
        public void HandleMoveInput(Vector2 input)
        {
            _state.MoveDirection = new Vector3(input.x, 0, input.y);
            _mover.Move();
        }

        public void HandleJumpInput()
        {
            if (_state.IsGrounded)
            {
                _state.IsJumping = true;
                _jumper.Jump();
            }
        }
    }
}