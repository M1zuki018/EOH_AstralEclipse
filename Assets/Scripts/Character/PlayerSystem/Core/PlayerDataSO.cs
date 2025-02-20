using UnityEngine;

namespace PlayerSystem.Core
{
    /// <summary>
    /// プレイヤーに必要な変数のうちゲーム中に変更されないものを設定する
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerData", menuName = "GameData/PlayerData")]
    public class PlayerDataSO : ScriptableObject
    {
        [Header("移動速度")]
        [SerializeField][Comment("走行スピード")] private float _runSpeed = 2f;
        [SerializeField][Comment("歩行スピード")] private float _walkSpeed = 1f;
        
        [Header("ジャンプ関連")]
        [SerializeField][Comment("ジャンプ力")] private float _jumpPower = 0.7f;
        [SerializeField][Comment("ジャンプ中の移動速度")] private float _jumpMoveSpeed = 2f;
        [SerializeField][Comment("重力")] private float _gravity = -17.5f;
        
        [Header("ステップ関連")]
        [SerializeField][Comment("ステップの最大回数")] private int _maxSteps = 10;
        [SerializeField][Comment("ステップ数の回復間隔")] private float _recoveryTime = 5f;
        
        [Header("カメラ関連")]
        [SerializeField][Comment("カメラが回る速さ")] private float _rotationSpeed = 10f;
        
        public float RunSpeed => _runSpeed;
        public float WalkSpeed => _walkSpeed;
        
        public float JumpPower => _jumpPower;
        public float JumpMoveSpeed => _jumpMoveSpeed;
        public float Gravity => _gravity;
        
        public int MaxSteps => _maxSteps;
        public float RecoveryTime => _recoveryTime;
        
        public float RotationSpeed => _rotationSpeed;
    }
}
