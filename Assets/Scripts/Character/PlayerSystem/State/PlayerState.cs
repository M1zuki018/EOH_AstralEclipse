using UnityEngine;

namespace PlayerSystem.State
{
    /// <summary>
    /// プレイヤーの各種状態をまとめたクラス
    /// </summary>
    public class PlayerState
    {
        /// <summary>入力された方向</summary>
        public Vector3 MoveDirection { get; set; }
        
        /// <summary>垂直方向の速度</summary>
        public Vector3 Velocity { get; set; }
        
        /// <summary>移動する速度</summary>
        public float MoveSpeed { get; set; }

        public bool IsWalking { get; set; } = true;　//歩いているか
        public bool IsGrounded { get; set; } //地面についているか
        public bool IsJumping { get; set; } //ジャンプ中か
        public bool IsCrouching { get; set; } //しゃがみ中か
        public bool IsClimbing { get; set; } //壁のぼり中か
        public bool IsVaulting { get; set; } //乗り越えアクション中か
        public bool IsGuarding { get; set; } //ガード中か
        public bool IsAttacking { get; set; } //攻撃中か

        public bool CanClimb { get; set; } //壁のぼりできるか
        public bool CanBigJump { get; set; } //大ジャンプできるか
        public bool CanVault { get; set; } //乗り越えできるか
        
        public Vector3 WallNormal { get; set; } //壁の法線
        
        public bool IsBossBattle { get; set; } //ボス戦中か
    }
}

