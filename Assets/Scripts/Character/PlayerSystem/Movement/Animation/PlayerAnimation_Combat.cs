using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// プレイヤーの攻撃関連のアニメーションを管理するクラス
/// </summary>
public class PlayerAnimation_Combat
{
    private Animator _animator;
    private PlayerBlackBoard _bb;

    public PlayerAnimation_Combat(Animator animator, PlayerBlackBoard bb)
    {
        _animator = animator;
        _bb = bb;
    }

    /// <summary>
    /// 攻撃アニメーションをトリガーする
    /// </summary>
    public void TriggerAttack() => _animator.SetTrigger("Attack"); 

    /// <summary>
    /// 臨戦状態アニメーションをトリガーする
    /// </summary>
    public void TriggerReadyForBattle() => _animator.SetTrigger("ReadyForBattle");

    /// <summary>
    /// スキルアニメーションをトリガーする
    /// </summary>
    public void UseSkill()
    {
        _animator.SetTrigger("Skill");
        _animator.SetInteger("SkillType", _bb.UsingSkillIndex - 1);
    }
}
