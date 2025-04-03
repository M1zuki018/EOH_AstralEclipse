using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem.Input
{
    /// <summary>
    /// PlayerのInputSystemを管理するクラス
    /// </summary>
    public class PlayerInputManager : ViewBase
    {
        [SerializeField, HighlightIfNull] private List<InputActionReference> _moveActions; //InputSystemのアクション参照
        public List<InputActionReference> MoveActions => _moveActions;
        
        private PlayerInput _playerInput; // PlayerInput コンポーネント
        
        // 入力情報
        private IPlayerInputReceiver _iPlayerInputReceiver; 
        public IPlayerInputReceiver IPlayerInputReceiver => _iPlayerInputReceiver;

        public override UniTask OnAwake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _iPlayerInputReceiver = new PlayerInputProcessor(GetComponent<PlayerBrain>().BB);
            RegisterInputActions(); // 入力の登録
            
            // ステートに合わせて入力制限をかけたり解除したりする処理
            GameManager.Instance.CurrentGameStateProp
                .Subscribe(newState => InputRestrictions(newState))
                .AddTo(this);
            
            return base.OnAwake();
        }

        private void OnDestroy()
        {
            UnregisterInputActions(); // クリーンアップ
        }

        /// <summary>
        /// PlayerInput の各アクションに対応するメソッドを登録
        /// </summary>
        private void RegisterInputActions()
        {
            _playerInput.actions["Fire"].performed += OnAttack;
            _playerInput.actions["Skill1"].performed += OnSkill1;
            _playerInput.actions["Skill2"].performed += OnSkill2;
            _playerInput.actions["Skill3"].performed += OnSkill3;
            _playerInput.actions["Skill4"].performed += OnSkill4;
            _playerInput.actions["Action"].performed += OnAction;
            _playerInput.actions["Move"].started += OnMove;
            _playerInput.actions["Move"].performed += OnMove;
            _playerInput.actions["Move"].canceled += OnMove;
            _playerInput.actions["Jump"].performed += OnJump;
            _playerInput.actions["Walk"].performed += OnWalk;
            _playerInput.actions["Step"].performed += OnStep;
            _playerInput.actions["Guard"].performed += OnGuard;
            _playerInput.actions["Guard"].canceled += OnGuard;
            _playerInput.actions["LockOn"].performed += OnLockOn;
            _playerInput.actions["Pause"].performed += OnPause;
        }

        /// <summary>
        /// PlayerInput の登録を解除（OnDestroy用）
        /// </summary>
        private void UnregisterInputActions()
        {
            _playerInput.actions["Fire"].performed -= OnAttack;
            _playerInput.actions["Skill1"].performed -= OnSkill1;
            _playerInput.actions["Skill2"].performed -= OnSkill2;
            _playerInput.actions["Skill3"].performed -= OnSkill3;
            _playerInput.actions["Skill4"].performed -= OnSkill4;
            _playerInput.actions["Action"].performed -= OnAction;
            _playerInput.actions["Move"].started -= OnMove;
            _playerInput.actions["Move"].performed -= OnMove;
            _playerInput.actions["Move"].canceled -= OnMove;
            _playerInput.actions["Guard"].performed -= OnGuard;
            _playerInput.actions["Guard"].canceled -= OnGuard;
            _playerInput.actions["LockOn"].performed -= OnLockOn;
            _playerInput.actions["Pause"].performed -= OnPause;
        }


        #region 入力されたときのメソッド一覧

        /// <summary>攻撃処理</summary>
        public void OnAttack(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandleAttackInput();

        /// <summary>スキル処理</summary>
        public void OnSkill1(InputAction.CallbackContext context) => HandleSkillInput(context, 1);

        public void OnSkill2(InputAction.CallbackContext context) => HandleSkillInput(context, 2);
        public void OnSkill3(InputAction.CallbackContext context) => HandleSkillInput(context, 3);
        public void OnSkill4(InputAction.CallbackContext context) => HandleSkillInput(context, 4);
        
        public void OnAction(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandleActionInput();

        /// <summary>移動処理</summary>
        public void OnMove(InputAction.CallbackContext context) =>
            _iPlayerInputReceiver.HandleMoveInput(context.ReadValue<Vector2>());

        /// <summary>ジャンプ処理</summary>
        public void OnJump(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandleJumpInput();

        /// <summary>歩きと走り状態を切り替える</summary>
        public void OnWalk(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandleWalkInput();

        /// <summary>ステップ</summary>
        public void OnStep(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandleStepInput();

        /// <summary>ガード状態を切り替える</summary>
        public void OnGuard(InputAction.CallbackContext context) => HandleGuardInput(context);

        /// <summary>ロックオン機能</summary>
        public void OnLockOn(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandleLockOnInput();

        public void OnPause(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandlePauseInput();

        #endregion

        #region 入力の条件文

        private void HandleSkillInput(InputAction.CallbackContext context, int index)
        {
            //index で スキル1~4のどのボタンを押されたか判断する
            if (context.performed) _iPlayerInputReceiver.HandleSkillInput(index);
        }

        private void HandleGuardInput(InputAction.CallbackContext context)
        {
            if (context.performed) _iPlayerInputReceiver.HandleGuardInput(true);
            if (context.canceled) _iPlayerInputReceiver.HandleGuardInput(false);
        }

        #endregion

        /// <summary>
        /// ステートを監視して入力制限をかける/解除する処理
        /// </summary>
        private void InputRestrictions(GameState newState)
        {
            if (newState == GameState.Movie || newState == GameState.Title)
            {
                _playerInput.DeactivateInput();
            }
            else
            {
                _playerInput.ActivateInput();
            }
        }
    }
}