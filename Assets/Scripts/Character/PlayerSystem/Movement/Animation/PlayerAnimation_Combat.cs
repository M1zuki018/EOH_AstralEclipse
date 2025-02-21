using UnityEngine;

/// <summary>
/// プレイヤーの攻撃関連のアニメーションを管理するクラス
/// </summary>
public class PlayerAnimation_Combat
{
    private Animator _animator;

    public PlayerAnimation_Combat(Animator animator)
    {
        _animator = animator;
    }

    /// <summary>
    /// 攻撃アニメーションをトリガーする
    /// </summary>
    public void TriggerAttack()
    {
        _animator.SetTrigger("Attack"); 
    }

    /// <summary>
    /// 臨戦状態アニメーションをトリガーする
    /// </summary>
    public void TriggerReadyForBattle()
    {
        _animator.SetTrigger("ReadyForBattle");
    }
}
