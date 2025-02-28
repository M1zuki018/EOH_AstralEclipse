using PlayerSystem.ActionFunction;
using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// ガード機能を提供する
/// </summary>
public class GuardFunction : IGuardable
{
    //ガードブレイク＝willの値が削り切られてしまったらブレイク状態

    private PlayerBlackBoard _bb;
    private Animator _animator;

    public GuardFunction(PlayerBlackBoard bb, Animator animator)
    {
        _bb = bb;
        _animator = animator;
    }

    /// <summary>
    /// ガードし始めたときの処理
    /// </summary>
    public void GuardStart()
    {
        _animator.SetBool("Guard", true);
        _bb.IsGuarding = true;
    }

    /// <summary>
    /// ガード中の処理
    /// </summary>
    public void Guard()
    {
        Debug.Log("ガード中");
        
    }

    /// <summary>
    /// ガードをやめるときの処理
    /// </summary>
    public void GuardEnd()
    {
        _animator.SetBool("Guard", false);
        _bb.IsGuarding = false;
    }
}
