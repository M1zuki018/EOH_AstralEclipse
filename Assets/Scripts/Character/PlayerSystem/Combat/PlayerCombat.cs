using System.Collections.Generic;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// プレイヤーの攻撃に関する処理
/// </summary>
public class PlayerCombat : MonoBehaviour, ICombat
{
    public int BaseAttackPower { get; private set; } = 10; //攻撃力
    public int TP { get; private set; } = 100; //TP
    public AttackHitDetector Detector { get; private set; }
    private PlayerMovement _playerMovement;
    private DamageHandler _damageHandler;
    public UIManager _uiManager;
    [SerializeField] private SkillSO _skillSet;
    public SkillSO SkillSet => _skillSet;

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
    public void UseSkill(int index)
    {
        SkillData skill = _skillSet.Cast(index); //スキルデータを取得する

        if (TP < skill.ResourceCost) //TPの判定を行う
        {
            return;
        }

        //発動条件がセットされているとき、条件が満たされていない場合は発動しない
        if(skill.CastCondition != null && !skill.CastCondition.IsSatisfied())
        {
            Debug.Log($"{skill.Name} の発動条件が満たされていません");
            return;
        }
        
        List<IDamageable> damageables = Detector.PerformAttack();
        foreach (IDamageable damageable in damageables)
        {
            _damageHandler.ApplyDamage(damageable, 
                baseDamage: Mathf.FloorToInt(BaseAttackPower * skill.AttackMultiplier), //攻撃力*スキル倍率。小数点以下切り捨て
                 0, gameObject);
        }
        
        _uiManager.UpdatePlayerTP(skill.ResourceCost);
        Debug.Log($"スキルを使った　発動：{skill.Name}");
    }
}