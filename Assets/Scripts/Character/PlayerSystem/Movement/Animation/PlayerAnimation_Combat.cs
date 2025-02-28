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
    public void TriggerAttack()
    {
        if(_bb.IsGrounded) // 地面にいたら通常攻撃
            _animator.SetTrigger("Attack"); 
        else 
            _animator.SetTrigger("AttackAir");
    }

    /// <summary>
    /// 臨戦状態アニメーションをトリガーする
    /// </summary>
    public void TriggerReadyForBattle()
    {
        _animator.SetTrigger("ReadyForBattle");
    }
}
