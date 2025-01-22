using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// プレイヤーの攻撃に関する処理
/// </summary>
public class PlayerCombat : MonoBehaviour, ICombat
{
    public int BaseAttackPower { get; private set; } = 10; //攻撃力
    private PlayerMovement _playerMovement;
    private DamageHandler _damageHandler;

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _damageHandler = new DamageHandler();
    }
    
    /// <summary>
    /// 攻撃処理
    /// </summary>
    public void Attack(IDamageable target)
    {
        _playerMovement._animator.SetTrigger("Attack"); //アニメーションのAttackをトリガーする
        //TODO: 処理を実装する
        _damageHandler.ApplyDamage(target, BaseAttackPower, 0, gameObject);
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