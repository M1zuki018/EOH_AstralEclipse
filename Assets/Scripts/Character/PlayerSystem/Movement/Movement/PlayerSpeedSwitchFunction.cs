using System;
using PlayerSystem.State;
using UniRx;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// プレイヤーの歩き/走りの切り替え処理を行うクラス
    /// </summary>
    public class PlayerSpeedSwitchFunction : ISpeedSwitchable
    {
        private PlayerBlackBoard _bb;
        
        private IDisposable _walkChangedSubscription;
        
        public PlayerSpeedSwitchFunction(PlayerBlackBoard bb)
        {
            _bb = bb;
            _bb.MoveSpeed = _bb.Data.WalkSpeed;
        }
        
        /// <summary>
        /// 動きの速度を切り替える実装
        /// </summary>
        public void Walk()
        {
            // 黒板のWalkingのbool値が変更されたとき、移動速度を変更する
            _walkChangedSubscription = _bb.IsWalking
                .DistinctUntilChanged()
                .Subscribe(_ => _bb.MoveSpeed = _bb.IsWalking.Value ? _bb.Data.WalkSpeed : _bb.Data.RunSpeed);
        }

        /// <summary>
        /// 購読解除
        /// </summary>
        public void DisposeWalkSubscription()
        {
            _walkChangedSubscription?.Dispose();
        }
    }
}
