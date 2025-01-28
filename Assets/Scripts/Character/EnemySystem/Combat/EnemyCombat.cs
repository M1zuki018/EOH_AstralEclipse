using System;
using System.Collections.Generic;
using PlayerSystem.Fight;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

/// <summary>
/// 敵の攻撃に関する処理
/// </summary>
public class EnemyCombat : MonoBehaviour, ICombat
{
    [SerializeField] private int _attackDamage = 5; //攻撃力
    [SerializeField] private AdjustDirection _adjustDirection;
    public AdjustDirection AdjustDirection => _adjustDirection;  
    public AttackHitDetector Detector { get; private set; }
    [SerializeField, Comment("攻撃間隔")] private float _attackCooldown = 1.5f;
    private EnemyBrain _brain;
    private DamageHandler _damageHandler;
    private float _attackTimer;

    private void Awake()
    {
        _brain = GetComponent<EnemyBrain>();
        _damageHandler = new DamageHandler();
        Detector = GetComponentInChildren<AttackHitDetector>();
    }

    public int BaseAttackPower => _attackDamage;
    
    /// <summary>
    /// 攻撃状態の処理
    /// </summary>
    public void HandleAttackState(Transform target, Action OnAttackEnd, float attackRange)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        
        if (distanceToPlayer > attackRange)
        {
            OnAttackEnd?.Invoke(); //攻撃状態から追跡状態に遷移する
            return;
        }
        
        // 攻撃処理
        _attackTimer -= Time.deltaTime; //クールタイムをTime.deltaTimeごとに減らしていくような計算方法
        if (_attackTimer <= 0f)
        {
            Attack();
            _attackTimer = _attackCooldown + Random.Range(-1f, 1f); //攻撃間隔にランダム性を持たせる
        }
    }
    
    /// <summary>
    /// 攻撃処理
    /// </summary>
    public void Attack()
    {
        _brain.Animator.SetInteger("AttackType", Random.Range(0, 2)); //攻撃の抽選
        _brain.Animator.SetTrigger("Attack");　//アニメーションのAttackをトリガーする
    }
    
    public void PerformAttack(int index)
    {
        Detector.AttackType = index;
        List<IDamageable> damageables = Detector.PerformAttack();
        Debug.Log(damageables.Count);
        foreach (IDamageable damageable in damageables)
        {
            _damageHandler.ApplyDamage(damageable, BaseAttackPower, 0, gameObject);
        }
    }

    /// <summary>
    /// スキル処理
    /// </summary>
    public void UseSkill(int index)
    {
        Debug.Log($"{gameObject.name} がスキルを使った　発動： {index}");
        //TODO: スキルの処理を実装する
    }

    
}