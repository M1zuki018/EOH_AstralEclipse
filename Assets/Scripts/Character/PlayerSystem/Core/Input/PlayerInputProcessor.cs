using System;
using UnityEngine;
using PlayerSystem.State;

namespace PlayerSystem.Input
{
    /// <summary>
    /// プレイヤーの各入力ごとに呼び出す処理をまとめているクラス
    /// </summary>
    public class PlayerInputProcessor : IPlayerInputReceiver
    {
        #region フィールドと初期化
        
        private PlayerBlackBoard _bb;
        private InputBuffer _inputBuffer;
        public InputBuffer InputBuffer => _inputBuffer;
        
        public event Action OnLockOn;
        
        /// <summary>
        /// 初期化
        /// </summary>
        public PlayerInputProcessor(PlayerBlackBoard blackBoard)
        {
            _bb = blackBoard;
            _inputBuffer = new InputBuffer();
        }

        #endregion
        

        // ここからステートマシンを介したあと、ActionHandlerから処理を行う

        #region 黒板を書き換えて、PlayerControllerのFixedUpdateで動くもの

        /// <summary>移動入力処理。Vector3への変換だけ行う</summary>
        public void HandleMoveInput(Vector2 input)
        {
            _bb.MoveDirection = new Vector3(input.x, 0, input.y);
        }
        
        /// <summary>歩き状態にする入力処理</summary>
        public void HandleWalkInput() => _bb.IsWalking.Value = !_bb.IsWalking.Value;

        #endregion

        #region InputBufferを介さないもの（すぐにアクションを返していいもの）

        /// <summary>ポーズ入力処理</summary>
        public void HandlePauseInput()
        {
            //TODO: ポーズ機能の実装を書く
            throw new System.NotImplementedException();
        }
        
        /// <summary>ロックオン入力処理</summary>
        public void HandleLockOnInput() => OnLockOn?.Invoke();

        #endregion
        
        #region InputBufferを使って記録しておくもの

        /// <summary>ジャンプ入力処理</summary>
        public void HandleJumpInput() => _inputBuffer.AddInput(InputNameEnum.Jump);
        
        /// <summary>ステップ入力処理</summary>
        public void HandleStepInput() => _inputBuffer.AddInput(InputNameEnum.Step);

        /// <summary>ガード入力処理</summary>
        public void HandleGuardInput(bool input) => _inputBuffer.AddInput(InputNameEnum.Guard);
        
        /// <summary>通常攻撃の入力処理</summary>
        public void HandleAttackInput() => _inputBuffer.AddInput(InputNameEnum.Attack);

        /// <summary>スキル攻撃の入力処理</summary>
        public void HandleSkillInput(int index)
        {
            _bb.UsingSkillIndex = index;
            _inputBuffer.AddInput(InputNameEnum.Skill);
        }
        
        /// <summary>特殊アクション・刀投げ/回収の入力処理</summary>
        public void HandleActionInput() => _inputBuffer.AddInput(InputNameEnum.Action);
        
        #endregion
    }
}