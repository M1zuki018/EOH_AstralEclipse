using PlayerSystem.ActionFunction;
using UnityEngine;

/// <summary>
/// ガード機能を提供する
/// </summary>
public class GaudeFunction : MonoBehaviour, IGaudeable
{
    //ガードの耐久値＝will
    //押している間だけガード出来る→押した瞬間にtrue、放した時にfalse
    //ガードをしている間は、HPではなくwillが削れる
    //ガードブレイク＝willの値が削り切られてしまったらブレイク状態
    
    
    public void Gaude(bool input)
    {
        //TODO:実装を書く
    }
}
