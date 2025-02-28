using System;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// プレイヤーのHPを管理するクラス
/// </summary>
public class Health : MonoBehaviour, IHealth
{
    public int MaxHP => _brain.BB.Status.MaxHP; // 最大HP
    public int CurrentHP => _brain.BB.CurrentHP; // 現在のHP
    public int MaxWill => _brain.BB.Status.Will; // 最大Will
    public int CurrentWill => _brain.BB.CurrentWill; // 現在のWill
    public bool IsDead => CurrentHP <= 0; //HPが0以下になったら死亡する
    public event Action<int, GameObject> OnDamaged; //ダメージを受けた時のイベント
    public event Action<int, GameObject> OnHealed; //回復イベント
    public event Action<GameObject> OnDeath; //死亡イベント

    private PlayerBrain _brain;
    
    private void Awake()
    { 
        _brain = GetComponent<PlayerBrain>();

        //初期化する
        _brain.BB.CurrentWill = _brain.BB.Status.Will;
        
        _brain.BB.CurrentHP = MaxHP;
        _brain.BB.CurrentWill = MaxWill;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) TakeDamage(25, gameObject);
    }

    [ContextMenu("ダメ―ジテスト")]
    private void Test()
    {
        _brain.BB.ParryReception = true;
        TakeDamage(25, gameObject);
    }
    
    /// <summary>
    /// ダメージを受ける処理の実装
    /// </summary>
    public void TakeDamage(int amount, GameObject attacker)
    {
        if (_brain.BB.ParryReception)
        {
            // パリィ成功。これ以降の処理は行わない
            Debug.Log("SuccessParry : パリィ成功");
            _brain.BB.SuccessParry = true;
            return;
        }
        
        if(IsDead) return; //死亡状態ならこれ以降の処理は行わない
        
        //if(attacker.CompareTag(tag)) return;

        if(_brain.BB.IsSteping) return; //ステップ中の場合、ダメージを受けない

        // ガード中はWillを削る
        if (_brain.BB.IsGuarding)
            _brain.BB.CurrentWill -= amount;
        else
            _brain.BB.CurrentHP -= amount;
        
        OnDamaged?.Invoke(amount, attacker); //ダメージイベント発火

        if (IsDead) //死亡判定
        {
            OnDeath?.Invoke(attacker); //死亡イベント発火
        }
    }

    /// <summary>
    /// 回復処理
    /// </summary>
    public void Heal(int amount, GameObject healer)
    {
        if(IsDead) return; //死亡状態ならこれ以降の処理は行わない
        
        _brain.BB.CurrentHP += amount;
        OnHealed?.Invoke(amount, healer);
    }
}
