using System.Collections.Generic;
using PlayerSystem.Animation;
using PlayerSystem.Core;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem.State
{
    /// <summary>
    /// プレイヤーの各種状態をまとめたクラス
    /// </summary>
    public class PlayerBlackBoard
    {
        private PlayerDataSO _data;
        private PlayerStatusSO _status;

        public PlayerBlackBoard(PlayerDataSO data, PlayerStatusSO status, Input.PlayerInputManager inputManager)
        {
            _data = data;
            _status = status;
            MoveActions = inputManager.MoveActions;
        }
        
        /// <summary>定数</summary>
        public PlayerDataSO Data => _data;
        
        /// <summary>ステータス</summary>
        public PlayerStatusSO Status => _status;
        
        /// <summary>現在のHP</summary>
        public int CurrentHP { get; set; }
        
        /// <summary>現在のTP</summary>
        public int CurrentTP { get; set; }
        
        /// <summary>現在のWill</summary>
        public int CurrentWill { get; set; }
        
        /// <summary>発動しているスキルのIndex</summary>
        public int UsingSkillIndex { get; set; }
        
        /// <summary>入力された方向</summary>
        public Vector3 MoveDirection { get; set; }
        
        /// <summary>垂直方向の速度</summary>
        public Vector3 Velocity { get; set; }
        
        /// <summary>カメラの向きに合わせて補正された後の移動方向</summary>
        public Vector3 CorrectedDirection { get; set; }
        
        /// <summary>移動する速度</summary>
        public float MoveSpeed { get; set; }

        public ReactiveProperty<bool> IsWalking { get; set; } = new ReactiveProperty<bool>(true);　//歩いているか
        public bool IsGrounded { get; set; } //地面についているか
        public bool IsJumping { get; set; } //ジャンプ中か

        public bool IsGuarding { get; set; } //ガード中か
        public bool IsAttacking { get; set; } //攻撃中か
        public bool IsSteping { get; set; } //ステップ中か
        
        public bool IsBossBattle { get; set; } //ボス戦中か
        public bool DebugMode { get; set; } //デバッグ中か
        
        public bool ApplyGravity{ get; set; } = false; // 重力を適用するか
        
        public int CurrentSteps { get; set; } //現在のステップ数

        /// <summary>プレイヤーの移動に関するアクションリファレンス</summary>
        public List<InputActionReference> MoveActions { get; }
        
        /// <summary>アニメーションを制御するクラス</summary>
        public PlayerAnimationController AnimController { get; set; }
    }
}

