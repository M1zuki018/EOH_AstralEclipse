using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// プレイヤーに重力を加える処理
/// </summary>
public class PlayerGravity
{
    private PlayerBlackBoard _bb;
    private CharacterController _cc;
    
    private readonly float _gravity = -17.5f;

    public PlayerGravity(PlayerBlackBoard bb, CharacterController cc)
    {
        _bb = bb;
        _cc = cc;
    }
    
    /// <summary>
    /// 重力を適用する
    /// </summary>
    public void ApplyGravity()
    {
        if (_bb.ApplyGravity)
        {
            Vector3 velocity = _bb.Velocity;
            velocity.y += _gravity * Time.deltaTime;
            _bb.Velocity = velocity;
            _cc.Move(_bb.Velocity * Time.deltaTime); // 垂直方向の速度を反映
        }
    }
}
