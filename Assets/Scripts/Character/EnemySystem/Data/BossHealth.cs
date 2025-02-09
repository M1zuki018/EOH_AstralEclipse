using System;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// ボス用のHealthクラス
/// </summary>
public class BossHealth : MonoBehaviour, IHealth
{
    [SerializeField] private int _maxHPData = 300;
    [SerializeField] private int _breakAmountData = 150;
    [SerializeField] private BossMover _bossMover;
    
    public int MaxHP { get; private set; } //最大HP
    public int CurrentHP { get; private set; } //現在のHP
    
    public int BreakAmount{ get; private set; } //DPSチェックの最大値
    
    public int CurrentBreakAmount{ get; private set; } //DPSチェックの現在の値
    
    public bool IsDead => CurrentHP <= 0;
    
    public bool IsCheckComplete => CurrentBreakAmount <= 0; //DPSチェックの値が削り切ったか

    public event Action<int, GameObject> OnDamaged;
    
    public event Action<int, GameObject> OnHealed;
    
    public event Action<GameObject> OnDeath;
    
    public event Action<GameObject> OnCheckComplete; //DPSチェック終了時に呼び出すイベント
    
    private void Awake()
    {
        MaxHP = _maxHPData;
        CurrentHP = _maxHPData; //HPを初期化する
        BreakAmount = _breakAmountData;
        CurrentBreakAmount = _breakAmountData; //DPSチェック用の値を初期化する
    }
    
    public void TakeDamage(int amount, GameObject attacker)
    {
        if(IsDead) return; //死亡状態ならこれ以降の処理は行わない
        
        if(attacker.tag == this.tag) return;
        
        if (!_bossMover.IsDPSCheak)
        {
            //DPSチェック中でなければ通常のダメージイベントを行う
            CurrentHP -= amount;
            OnDamaged?.Invoke(amount, attacker); //ダメージイベント発火

            if (IsDead) //死亡判定
            {
                OnDeath?.Invoke(attacker); //死亡イベント発火
            }
        }
        else
        {
            //DPSチェック中なら専用の値を変化させる
            CurrentBreakAmount -= amount;
            UIManager.Instance.UpdateBossDpsSlider(CurrentBreakAmount); //スライダーを更新

            if (IsCheckComplete)
            {
                OnCheckComplete?.Invoke(attacker);
            }
        }
        
    }

    
    public void Heal(int amount, GameObject healer)
    {
        if(IsDead) return; //死亡状態ならこれ以降の処理は行わない
        
        CurrentHP += amount;
        OnHealed?.Invoke(amount, healer);
    }

    
}
