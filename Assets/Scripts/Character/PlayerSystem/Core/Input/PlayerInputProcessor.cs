using System;
using PlayerSystem.ActionFunction;
using UnityEngine;
using PlayerSystem.Movement;
using PlayerSystem.State;
using UniRx;
using UnityEngine.InputSystem;

namespace PlayerSystem.Input
{
    /// <summary>
    /// プレイヤーの各入力ごとに呼び出す処理をまとめているクラス
    /// </summary>
    public class PlayerInputProcessor : IPlayerInputReceiver
    {
        #region フィールドと初期化

        private readonly PlayerBlackBoard _blackBoard;

        /// <summary>
        /// 初期化
        /// </summary>
        public PlayerInputProcessor(PlayerBlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
        }

        #endregion

        public event Action OnJump;
        public event Action OnStep;
        public event Action OnGuard;
        public event Action OnLockOn;
        public event Action OnAttack;
        public event Action<int> OnSkill;


        // 以降、動作はインターフェースを介して行う


        /// <summary>移動入力処理</summary>
        public void HandleMoveInput(Vector2 input)
        {
            _blackBoard.MoveDirection = new Vector3(input.x, 0, input.y);
            //_actionHandler.Move();
        }

        /// <summary>ジャンプ入力処理</summary>
        public void HandleJumpInput() => OnJump?.Invoke();

        /// <summary>歩き状態にする入力処理</summary>
        public void HandleWalkInput() => throw new System.NotImplementedException();
        //_walker.Walk();

        /// <summary>ポーズ入力処理</summary>
        public void HandlePauseInput()
        {
            //TODO: ポーズ機能の実装を書く
            throw new System.NotImplementedException();
        }

        /// <summary>ステップ入力処理</summary>
        public void HandleStepInput() => OnStep?.Invoke();

        /// <summary>ガード入力処理</summary>
        public void HandleGuardInput(bool input) => OnGuard?.Invoke();

        /// <summary>ロックオン入力処理</summary>
        public void HandleLockOnInput() => OnLockOn?.Invoke();

        /// <summary>通常攻撃の入力処理</summary>
        public void HandleAttackInput() => OnAttack?.Invoke();

        /// <summary>スキル攻撃の入力処理</summary>
        public void HandleSkillInput(int index) => OnSkill?.Invoke(index);
    }
}