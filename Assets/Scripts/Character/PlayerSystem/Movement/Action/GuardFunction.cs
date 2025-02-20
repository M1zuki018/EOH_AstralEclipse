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

    public GuardFunction(PlayerBlackBoard bb)
    {
        _bb = bb;
    }
    
    public void Gaude()
    {
        Debug.Log("ガード中");
        //TODO:実装を書く
    }
}
