using UnityEngine;
using PlayerSystem.State;

/// <summary>
/// 壁のぼり中の判定を行う
/// </summary>
public class WallChecker : MonoBehaviour
{
    [SerializeField, Comment("キャラクターの目の前に壁があるか判定する高さ")] private float _wallCheckOffset;
    [SerializeField, Comment("よじのぼり用のRayの高さ")] private float _upperWallCheckOffset;
    [SerializeField, Comment("Rayの長さ")] private float _wallCheckDistance;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField, HighlightIfNull] private ClimbingIK _climbingIK;

    private bool _isForwardWall;
    private bool _isUpperWall;
    private bool _isGrab;

    /// <summary>崖つかみ中か</summary>
    public bool IsGrab => _isGrab;
    
    private void Update()
    {
        if (_playerMovement.IsClimbing) //壁のぼり中だけ処理を行う
        {
            //  壁判定に使用するRay
            Ray wallCheckRay = new Ray(transform.position + Vector3.up * _wallCheckOffset, transform.forward);
            Ray upperCheckRay = new Ray(transform.position + Vector3.up * _upperWallCheckOffset, transform.forward);

            //  壁判定を格納
            _isForwardWall = Physics.Raycast(wallCheckRay, _wallCheckDistance);
            _isUpperWall = Physics.Raycast(upperCheckRay, _wallCheckDistance);
            
            _isGrab = _isForwardWall && !_isUpperWall;
            _climbingIK.IkActive = _playerMovement.IsClimbing;
            
            /*
            // IKにターゲットをセットする
            if (_isForwardWall)
            {
                    _climbingIK.LeftHandTarget = CreateTemporaryTargetTransform(_climbingIK.LeftHandTarget.position);
                    _climbingIK.RightHandTarget = CreateTemporaryTargetTransform(_climbingIK.RightHandTarget.position);
                    _climbingIK.LeftFootTarget = CreateTemporaryTargetTransform(_climbingIK.LeftFootTarget.position);
                    _climbingIK.RightFootTarget = CreateTemporaryTargetTransform(_climbingIK.RightFootTarget.position);
            }
            */
        }
        else
        {
            _isGrab = false;
            _climbingIK.IkActive = false;
        }
    }

    /// <summary>
    /// IKのための処理
    /// </summary>
    private Transform CreateTemporaryTargetTransform(Vector3 origin)
    {
        RaycastHit hit;
        Ray wallRay = new Ray(transform.position + Vector3.up * _wallCheckOffset, transform.forward);

        if (Physics.Raycast(wallRay, out hit, _wallCheckDistance))
        {
            Transform tmp = hit.collider.transform;
            origin.x = tmp.position.x;
            origin.z = tmp.position.z;
            tmp.position = origin;
            return tmp;
        }

        return null;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = _isForwardWall ? Color.blue : Color.magenta;
        Gizmos.DrawRay(transform.position + Vector3.up * _wallCheckOffset, transform.forward * _wallCheckDistance);
        Gizmos.color = _isUpperWall ?  Color.blue : Color.magenta;
        Gizmos.DrawRay(transform.position + Vector3.up * _upperWallCheckOffset, transform.forward * _wallCheckDistance);
    }
}
