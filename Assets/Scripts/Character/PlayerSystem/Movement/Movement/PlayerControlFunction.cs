using System;
using Cinemachine;
using PlayerSystem.State;
using UniRx;
using UnityEngine;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// プレイヤーの移動・ジャンプ・歩き/走りの切り替え処理を包括したクラス
    /// </summary>
    public class PlayerControlFunction : IMovable, IJumpable, IWalkable
    {
        private PlayerBlackBoard _blackBoard;
        private Vector3 _moveNormal;
        
        private PlayerMovement _movement;
        private PlayerJump _jump;
        
        private IDisposable _walkChangedSubscription;

        private readonly float _runSpeed = 2f;
        private readonly float _walkSpeed = 1f;
        private readonly float _jumpPower = 0.7f;
        private readonly float _jumpMoveSpeed = 2f; //ジャンプ中の移動速度
        private readonly float _gravity = -17.5f;
        private readonly float _rotationSpeed = 10f;
        private readonly float _climbSpeed = 3f;
        
        public PlayerControlFunction(
            PlayerMovement movement, PlayerJump jump, PlayerBlackBoard blackBoard)
        {
            _movement = movement;
            _jump = jump;
            _blackBoard = blackBoard;
            _blackBoard.MoveSpeed = _walkSpeed;
        }
        
        public void Move() => _movement.Move(); // 移動
        
        public void Jump() => _jump.Jump(); // ジャンプ

        public void Jumping() => _jump.Jumping(); // ジャンプ中

        /// <summary>
        /// 動きの速度を切り替える実装
        /// </summary>
        public void Walk()
        {
            // 黒板のWalkingのbool値が変更されたとき、移動速度を変更する
            _walkChangedSubscription = _blackBoard.IsWalking
                .DistinctUntilChanged()
                .Subscribe(_ => _blackBoard.MoveSpeed = _blackBoard.IsWalking.Value ? _walkSpeed : _runSpeed);
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
