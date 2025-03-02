using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// 接地判定を管理するクラス
/// </summary>
public class GroundTrigger
{
    private readonly PlayerBlackBoard _bb;
    private readonly Transform _player;
    private RaycastHit _hit;

    public GroundTrigger(PlayerBlackBoard bb, Transform player)
    {
        _bb = bb;
        _player = player;
    }
    
    /// <summary>
    /// 接地判定
    /// </summary>
    public void CheckGrounded()
    {
        // 放つ光線の初期位置と姿勢
        // 若干身体にめり込ませた位置から発射しないと正しく判定できない時がある
        if (Physics.Raycast(
                origin: _player.position + Vector3.up * _bb.Settings.RayOffset,
                direction: Vector3.down,
                out _hit, _bb.Settings.RayLength, _bb.Settings.LayerMask, QueryTriggerInteraction.Ignore))
        {
            _bb.IsGrounded = _hit.collider.gameObject.CompareTag("Ground");
        }
        else
        {
            _bb.IsGrounded = false;
        }
    }
}
