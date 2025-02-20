using UniRx;
using UnityEngine;

namespace PlayerSystem.State
{
    /// <summary>
    /// プレイヤーの各種状態をまとめたクラス
    /// </summary>
    public class PlayerBlackBoard
    {
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
    }
}

