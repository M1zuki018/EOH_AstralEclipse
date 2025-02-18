using UnityEngine;

namespace PlayerSystem.Input
{
    /// <summary>
    /// 入力処理を管理するインターフェース
    /// </summary>
    public interface IInputHandler
    {
        #region 基本動作（PlayerSystem.Movement）
        void HandleMoveInput(Vector2 input);
        void HandleJumpInput();
        void HandleWalkInput();
        void HandlePauseInput();
        #endregion

        #region アクション（PlayerSystem.ActionFunction）
        void HandleStepInput();
        void HandleGaudeInput(bool input);
        void HandleLockOnInput();
        #endregion

        #region 戦闘（PlayerSystem.Fight）
        void HandleAttackInput();
        void HandleSkillInput(int index);
        #endregion
        
    }
}