using UnityEngine;

/// <summary>
/// 接地判定を管理するクラス
/// </summary>
public class GroundTrigger : MonoBehaviour
{
    [SerializeField] private float _rayLength = 1f; // Rayの長さ
    [SerializeField] private float _rayOffset; // Rayをどれくらい身体にめり込ませるか
    [SerializeField] private LayerMask _layerMask = default; // Rayの判定に用いるLayer
    [SerializeField] private PlayerBrain _brain;
    private RaycastHit _hit;

    /// <summary>
    /// 接地判定
    /// </summary>
    public void CheckGrounded()
    {
        // 放つ光線の初期位置と姿勢
        // 若干身体にめり込ませた位置から発射しないと正しく判定できない時がある
        if (Physics.Raycast(transform.position + Vector3.up * _rayOffset, Vector3.down,
                out _hit, _rayLength, _layerMask, QueryTriggerInteraction.Ignore))
        {
            if (_hit.collider.gameObject.CompareTag("Ground"))
            {
                _brain.BB.IsGrounded = true;
            }
        }
        else
        {
            _brain.BB.IsGrounded = false;
        }
    }
}
