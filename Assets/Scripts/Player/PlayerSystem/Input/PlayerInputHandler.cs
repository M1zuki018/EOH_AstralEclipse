using PlayerSystem.ActionFunction;
using UnityEngine;
using PlayerSystem.Movement;
using PlayerSystem.State;

namespace PlayerSystem.Input
{
    public class PlayerInputHandler : IInputHandler
    {
        #region 初期化と構造体
        
        private PlayerState _state;
        private PlayerMover _playerMover;
        private IMovable _mover;
        private IJumpable _jumper;
        private IWalkable _walker;
        private ICrouchable _croucher;
        private ISteppable _stepper;
        private IGaudeable _gauder;
        private ILockOnable _locker;
        private IWallRunable _wallruner;
        private IClimbale _climber;
        private IBigJumpable _bigjumper;
        private IVaultable _vaulter;

        public PlayerInputHandler(PlayerState state, IMovable mover, IJumpable jumper, IWalkable walker, ICrouchable croucher, 
            ISteppable steppable, IGaudeable gauder, ILockOnable locker, IWallRunable wallruner,
            IClimbale climbale, IBigJumpable bigjumper, IVaultable vaulter)
        {
            _state = state;
            _mover = mover;
            _jumper = jumper;
            _walker = walker;
            _croucher = croucher;
            _stepper = steppable;
            _gauder = gauder;
            _locker = locker;
            _wallruner = wallruner;
            _climber = climbale;
            _bigjumper = bigjumper;
            _vaulter = vaulter;
        }
        #endregion
        
        /// 以降、動作はインターフェースを介して行う
        
        public void HandleMoveInput(Vector2 input)
        {
            if (_state.IsClimbing) //壁のぼり中なら
            {
                _state.MoveDirection = new Vector3(0, input.y, -input.x);
            }
            else
            {
                _state.MoveDirection = new Vector3(input.x, 0, input.y);
            }
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
        
        public void HandleWalkInput()
        {
            _walker.Walk();
        }

        public void HandleCrouchInput(bool input)
        {
            _croucher.Crouch(input);
        }

        public void HandleStepInput()
        {
            if (_stepper.TryUseStep())
            {
                _stepper.Step();
            }
        }

        public void HandleGaudeInput(bool input)
        {
            _gauder.Gaude(input);
        }

        public void HandleLockOnInput()
        {
            _locker.LockOn();
        }

        public void HandleVaultInput()
        {
            throw new System.NotImplementedException();
        }

        public void HandleBigJumpInput()
        {
            throw new System.NotImplementedException();
        }

        public void HandleClimbInput()
        {
            throw new System.NotImplementedException();
        }

        public void HandleWallRunInput()
        {
            throw new System.NotImplementedException();
        }
    }
}