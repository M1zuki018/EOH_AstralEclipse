using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 足元が地面/壁か判定する
/// </summary>
public class GroundTrigger : MonoBehaviour
{
    [SerializeField] private float _rayLength = 1f; // Rayの長さ
    [SerializeField] private float _rayOffset; // Rayをどれくらい身体にめり込ませるか
    [SerializeField] private LayerMask _layerMask = default; // Rayの判定に用いるLayer
    [FormerlySerializedAs("_playerMovement")] [SerializeField] PlayerController playerController;
    private RaycastHit _hit;
    
    private void FixedUpdate()
    {
        CheckGrounded();
    }

    /// <summary>
    /// 接地判定
    /// </summary>
    private void CheckGrounded()
    {
        // 放つ光線の初期位置と姿勢
        // 若干身体にめり込ませた位置から発射しないと正しく判定できない時がある
        if (Physics.Raycast(transform.position + Vector3.up * _rayOffset, Vector3.down,
                out _hit, _rayLength, _layerMask, QueryTriggerInteraction.Ignore))
        {
            if (_hit.collider.gameObject.CompareTag("Ground") || _hit.collider.gameObject.CompareTag("JumpObject"))
            {
                playerController.PlayerState.IsGrounded = true;
                //_playerMovement.PlayerState.IsClimbing = false;
            }
            else if (_hit.collider.gameObject.CompareTag("Wall"))
            {
                //_playerMovement.IsWall = true;
                playerController.PlayerState.IsGrounded = false;
            }
        }
        else
        {
            playerController.PlayerState.IsGrounded = false;
            //_playerMovement.IsWall = false;
        }
    }

    /*
    /// <summary>
    /// デバッグ用
    /// </summary>
    private void OnDrawGizmos()
    {
        // 接地判定時は緑、空中にいるときは赤にする
        Gizmos.color = _playerMovement.PlayerState.IsGrounded ? Color.green : Color.red; 
        Gizmos.DrawRay(transform.position + Vector3.up * _rayOffset, Vector3.down * _rayLength);
    }
    */
    
}
