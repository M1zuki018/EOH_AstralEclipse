using System;
using UnityEngine;

namespace PlayerSystem.Fight
{
    /// <summary>
    /// HPの増減や現在のHP状態を管理します
    /// </summary>
    public interface IHealth : IDamageable
    {
        int CurrentHP { get; } // HP の取得
        int MaxHP { get; }
        int CurrentWill { get; } // Willの取得
        int MaxWill { get; }
        
        /// <summary>死亡状態</summary>
        bool IsDead { get; }
        
        /// <summary>回復する</summary>
        void Heal(int amount, GameObject healer);

        /// <summary>ダメージを受けたときのイベント</summary>
        event Action<int, GameObject> OnDamaged;
        
        /// <summary>回復時のイベント</summary>
        event Action<int, GameObject> OnHealed;
        
        /// <summary>死亡時のイベント</summary>
        event Action<GameObject> OnDeath;
    }

}
