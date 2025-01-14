using UnityEngine;

/// <summary>
/// 大ジャンプ機能を提供する
/// </summary>
public class BigJumpFunction : MonoBehaviour
{
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private string _targetTag = "JumpTarget";
    private PlayerMovement _playerMovement;
    private bool _canJump = false;
    private bool _isJumping = false;
    private Transform _targetObj; //対象のオブジェクト
    [SerializeField] private float _startAnimTime = 0.083f;
    [SerializeField] private float _endAnimTime = 0.960f;
    private MatchTargetWeightMask _mask = new MatchTargetWeightMask(Vector3.one, 1f);
    
    public bool CanJump{ get { return _canJump; } }
    
    //視界内にオブジェクトがあるか判定する
    //Fキーを押してジャンプ
    //ジャンプの軌道の計算式
    //モーション中はMoveが効かない状態にする

    private void Update()
    {
        if (_isJumping)
        {
            BigJumpMatchTarget();
        }
        else
        {
            //判定はジャンプ中は行わない
            CanBigJumpCheck();
        }
    }

    /// <summary>
    /// ジャンプ対象のオブジェクトがあるかどうかの判定を行うRay
    /// </summary>
    private void CanBigJumpCheck()
    {
        //視界内にオブジェクトがあるか判定する
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance))
        {
            if (hit.collider.CompareTag(_targetTag))
            {
                Debug.Log("ジャンプ可能");
                _canJump = true;
                _targetObj = hit.transform;
            }
        }
    }

    /// <summary>
    /// 大ジャンプ機能
    /// </summary>
    public void HandleBigJump(PlayerMovement playerMovement)
    {
        _playerMovement = playerMovement;
        _playerMovement._animator.SetTrigger("BigJump");
        _isJumping = true;
    }

    /// <summary>
    /// animatorのMatchTarget()を実行
    /// </summary>
    private void BigJumpMatchTarget()
    {
        _playerMovement._animator.MatchTarget(
            _targetObj.position,
            _targetObj.rotation,
            AvatarTarget.LeftFoot,
            _mask,
            _startAnimTime,
            _endAnimTime);
    }
}
