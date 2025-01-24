using PlayerSystem.ActionFunction;
using PlayerSystem.Fight;
using UnityEngine;
using PlayerSystem.Movement;
using PlayerSystem.State;

namespace PlayerSystem.Input
{
    public class PlayerInputHandler : IInputHandler
    {
        #region フィールドと初期化
        
        private readonly PlayerMovement _playerMovement;
        private readonly PlayerState _state;
        private readonly IMovable _mover; //移動
        private readonly IJumpable _jumper;　//ジャンプ
        private readonly IWalkable _walker; //歩きと走りの切り替え
        private readonly ICrouchable _croucher; //しゃがみ
        private readonly ISteppable _stepper; //ステップ
        private readonly IGaudeable _gauder; //ガード
        private readonly ILockOnable _locker; //ロックオン
        private readonly IWallRunable _wallruner; //ウォールラン
        private readonly IClimbale _climber; //壁のぼり
        private readonly IBigJumpable _bigjumper; //大ジャンプ
        private readonly IVaultable _vaulter; //乗り越え
        private readonly PlayerCombat _combat; //アクション

        public PlayerInputHandler(PlayerMovement playerMovement, PlayerState state, IMovable mover, IJumpable jumper, 
            IWalkable walker, ICrouchable croucher, ISteppable steppable, IGaudeable gauder, ILockOnable locker, IWallRunable wallruner,
            IClimbale climbale, IBigJumpable bigjumper, IVaultable vaulter, PlayerCombat combat)
        {
            _playerMovement = playerMovement;
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
            _combat = combat;
        }
        #endregion
        
        // 以降、動作はインターフェースを介して行う
        
        /// <summary>移動入力処理</summary>
        public void HandleMoveInput(Vector2 input)
        {
            if (_state.IsClimbing) //壁のぼり中なら
            {
                _state.MoveDirection = new Vector3(0, input.y, input.x);
            }
            else //通常時
            {
                _state.MoveDirection = new Vector3(input.x, 0, input.y);
            }
            _mover.Move();
        }

        /// <summary>ジャンプ入力処理</summary>
        public void HandleJumpInput()
        {
            if (_state.IsGrounded)
            {
                _state.IsJumping = true;
                _jumper.Jump();
            }
        }
        
        /// <summary>歩き状態にする入力処理</summary>
        public void HandleWalkInput()
        {
            _walker.Walk();
        }

        /// <summary>しゃがみ入力処理</summary>
        public void HandleCrouchInput(bool input)
        {
            _croucher.Crouch(input);
        }

        /// <summary>ポーズ入力処理</summary>
        public void HandlePauseInput()
        {
            //TODO: ポーズ機能の実装を書く
            throw new System.NotImplementedException();
        }

        /// <summary>ステップ入力処理</summary>
        public void HandleStepInput()
        {
            _stepper.TryUseStep();
        }

        /// <summary>ガード入力処理</summary>
        public void HandleGaudeInput(bool input)
        {
            _gauder.Gaude(input);
        }

        /// <summary>ロックオン入力処理</summary>
        public void HandleLockOnInput()
        {
            _locker.LockOn();
        }

        /// <summary>乗り越え入力処理</summary>
        public void HandleVaultInput()
        {
            //TODO:実装を書く
            throw new System.NotImplementedException();
        }

        /// <summary>大ジャンプ入力処理</summary>
        public void HandleBigJumpInput()
        {
            //TODO:実装を書く
            throw new System.NotImplementedException();
        }

        /// <summary>壁のぼり開始の入力処理</summary>
        public void HandleClimbStartInput()
        {
            _climber.StartClimbing();
        }
        
        /// <summary>壁のぼり中の入力処理</summary>
        public void HandleClimbInput()
        {
            _climber.HandleClimbing();
        }

        /// <summary>壁のぼり終了の入力処理</summary>
        public void HandleClimbEndInput()
        {
            _climber.EndClimbing();
        }

        /// <summary>ウォールランの入力処理</summary>
        public void HandleWallRunInput()
        {
            //TODO:実装を書く
            throw new System.NotImplementedException();
        }

        /// <summary>通常攻撃の入力処理</summary>
        public void HandleAttackInput()
        {
            if (_playerMovement.InteractableItem != null && _playerMovement.InteractableItem.CanGet)
            {
                _playerMovement.InteractableItem.Interact(); //インタラクトできるものがあればそれを呼ぶ
            }
            else
            {
                _combat.Attack();
            }
        }

        /// <summary>スキル攻撃の入力処理</summary>
        public void HandleSkillInput(int index)
        {
            _combat._uiManager.SelectedSkillIcon(index);
            //TODO: _combat.UseSkill(index, ターゲットどうするか);
        }
    }
}