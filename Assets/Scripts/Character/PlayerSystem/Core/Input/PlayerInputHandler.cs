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
        private readonly ISteppable _stepper; //ステップ
        private readonly IGaudeable _gauder; //ガード
        private readonly ILockOnable _locker; //ロックオン
        private readonly PlayerCombat _combat; //アクション

        public PlayerInputHandler(PlayerMovement playerMovement, PlayerState state, IMovable mover, IJumpable jumper, 
            IWalkable walker, ISteppable steppable, IGaudeable gauder, ILockOnable locker, PlayerCombat combat)
        {
            _playerMovement = playerMovement;
            _state = state;
            _mover = mover;
            _jumper = jumper;
            _walker = walker;
            _stepper = steppable;
            _gauder = gauder;
            _locker = locker;
            _combat = combat;
        }
        #endregion
        
        // 以降、動作はインターフェースを介して行う

        /// <summary>移動入力処理</summary>
        public void HandleMoveInput(Vector2 input)
        {
            _state.MoveDirection = new Vector3(input.x, 0, input.y);
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
            UIManager.Instance.SelectedSkillIcon(index);
            _combat.UseSkill(index);
        }
    }
}