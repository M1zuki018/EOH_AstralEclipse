using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// プレイヤーの攻撃に関する処理
/// </summary>
public class PlayerCombat : MonoBehaviour, ICombat
{
    public int AttackDamage { get; private set; } = 10; //攻撃力
    private PlayerMovement _playerMovement;

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    public void Attack()
    {
        _playerMovement._animator.SetTrigger("Attack"); //アニメーションのAttackをトリガーする
        //TODO: 処理を実装する
    }

    /// <summary>
    /// スキル処理
    /// </summary>
    public void UseSkill(string skillID)
    {
        Debug.Log($"スキルを使った　発動：{skillID}");
        //TODO: 処理を実装する
    }
}