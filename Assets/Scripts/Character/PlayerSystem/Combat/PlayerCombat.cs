using System.Collections.Generic;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// プレイヤーの攻撃に関する処理
/// </summary>
public class PlayerCombat : MonoBehaviour, ICombat
{
    public int BaseAttackPower { get; private set; } = 10; //攻撃力
    public AttackHitDetector Detector { get; private set; }
    private PlayerMovement _playerMovement;
    private DamageHandler _damageHandler;

    private void Start()
    {
        //コンポーネントを取得する
        _playerMovement = GetComponent<PlayerMovement>();
        _damageHandler = new DamageHandler();
        Detector = GetComponentInChildren<AttackHitDetector>();
    }
    
    /// <summary>
    /// 攻撃入力を受けた時に呼び出される処理
    /// </summary>
    public void Attack()
    {
        _playerMovement._animator.SetTrigger("Attack"); //アニメーションのAttackをトリガーする
        List<IDamageable> damageables = Detector.PerformAttack();
        foreach (IDamageable damageable in damageables)
        {
            _damageHandler.ApplyDamage(damageable, BaseAttackPower, 0, gameObject);
        }
    }

    /// <summary>
    /// スキル処理
    /// </summary>
    public void UseSkill(int index, IDamageable target)
    {
        int skillDamage = BaseAttackPower * 2;
        _damageHandler.ApplyDamage(target, skillDamage, 0, gameObject);
        //TODO: 処理を実装する
        Debug.Log($"スキルを使った　発動：{index}");
    }
}