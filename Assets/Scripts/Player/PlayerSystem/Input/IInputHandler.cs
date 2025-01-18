using UnityEngine;

namespace PlayerSystem.Input
{
    /// <summary>
    /// 移動・ジャンプの入力処理を管理
    /// </summary>
    public interface IInputHandler
    {
        void HandleMoveInput(Vector2 input);
        void HandleJumpInput();
    }
}