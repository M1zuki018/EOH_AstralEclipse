using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// 敵の攻撃に関する処理
/// </summary>
public class EnemyCombat : MonoBehaviour, ICombat
{
    public int AttackDamage { get; private set; } = 5; //攻撃力
    private EnemyBrain _brain;

    private void Awake()
    {
        _brain = GetComponent<EnemyBrain>();
    }
    
    /// <summary>
    /// 攻撃処理
    /// </summary>
    public void Attack()
    {
        Debug.Log($"{gameObject.name} が攻撃した");
        _brain.Animator.SetTrigger("Attack");
        //TODO: 攻撃処理を実装する
    }

    /// <summary>
    /// スキル処理
    /// </summary>
    public void UseSkill(string skillID)
    {
        Debug.Log($"{gameObject.name} がスキルを使った　発動： {skillID}");
        //TODO: スキルの処理を実装する
    }
}