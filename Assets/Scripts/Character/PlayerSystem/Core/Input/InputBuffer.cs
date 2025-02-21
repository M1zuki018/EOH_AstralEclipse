using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem.Input
{
    /// <summary>
    /// InputBufferクラス
    /// </summary>
    public class InputBuffer
    {
        private Dictionary<string, float> _inputBuffer = new Dictionary<string, float>();
        private readonly float _bufferTime = 0.2f; // 入力を保存する時間（秒）

        /// <summary>
        /// ボタンが押されたときに呼ばれる
        /// </summary>
        public void AddInput(string inputName) => _inputBuffer[inputName] = Time.time; // 現在の時間を記録

        /// <summary>
        /// バッファが有効か確認する
        /// </summary>
        public bool GetBufferedInput(string actionName)
        {
            if (_inputBuffer.TryGetValue(actionName, out float inputTime))
            {
                if (Time.time - inputTime <= _bufferTime)
                {
                    _inputBuffer.Remove(actionName); // 使ったら削除
                    return true;
                }
            }
            return false;
        }
    }
}