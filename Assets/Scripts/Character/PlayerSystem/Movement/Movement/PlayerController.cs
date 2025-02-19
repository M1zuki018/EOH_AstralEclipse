using UnityEngine;
using Cinemachine;
using PlayerSystem.ActionFunction;
using PlayerSystem.Input;
using PlayerSystem.Movement;
using PlayerSystem.State;

/// <summary>
/// プレイヤーの移動・空中移動などの処理を行う機能
/// </summary>
public class PlayerController : MonoBehaviour, IMatchTarget
{ 
    [Header("コンポーネント")]
    [SerializeField][ReadOnlyOnRuntime] private Transform _playerTransform; // プレイヤーのTransform
    [SerializeField][ReadOnlyOnRuntime] private CinemachineVirtualCamera _playerCamera; // カメラ
    [SerializeField][ReadOnlyOnRuntime] private CharacterController _characterController;
    
    // Animator
    [SerializeField][ReadOnlyOnRuntime] private Animator _animator;
    public Animator Animator => _animator;
    
    // プレイヤーの状態
    private PlayerBlackBoard _playerBlackBoard;
    public PlayerBlackBoard PlayerBlackBoard => _playerBlackBoard;

    private Collider _collider;
    [SerializeField] private Transform _targetTransform;
    
    #region 各種機能
    private IMovable _mover; //移動
    private IJumpable _jumper; //ジャンプ
    private IWalkable _walker; //歩きと走り状態の切り替え
    private ISteppable _stepFunction; //ステップ
    private IGaudeable _gaudeFunction; //ガード
    private ILockOnable _lockOnFunction; //ロックオン
    #endregion
    
    private void Awake()
    {
        InitializeState();
        InitializeComponents();
        
        TryGetComponent(out _collider);
        Animator.keepAnimatorStateOnDisable = true;
        
        // ターゲットマッチングを行うStateMachineBehaviorに自身を登録する
        foreach (var smb in Animator.GetBehaviours<MatchPositionSMB>())
        {
            smb._target = this;
        }
    }

    private void InitializeState()
    {
        _playerBlackBoard = new PlayerBlackBoard();
    }
    
    private void InitializeComponents()
    {
        // 移動処理を包括したクラスのインスタンスを生成
        _mover = new PlayerControlFunction(_characterController, Animator, _playerBlackBoard, _playerCamera, GetComponent<TrailRenderer>());
        _jumper = (IJumpable) _mover;
        _walker = (IWalkable) _mover;
        
        Animator.applyRootMotion = true; //ルートモーションを有効化
    }
    
    private void FixedUpdate()
    {
        if (_playerBlackBoard.IsJumping)
        {
            _jumper.Jumping(); //ジャンプ処理
            HandleGroundedCheck();
            HandleFalling(); //落下中の判定
        }
        else
        {
            _mover.Move(); //移動処理
            HandleGroundedCheck();
            HandleFalling(); //落下中の判定
        }
    }
    
    /// <summary>
    /// 地面にいるときの処理
    /// </summary>
    private void HandleGroundedCheck()
    {
        if (_playerBlackBoard.IsGrounded && _playerBlackBoard.Velocity.y < 0)
        {
            _playerBlackBoard.IsJumping = false;
            _playerBlackBoard.Velocity = new Vector3(0, -0.1f, 0); //確実に地面につくように少し下向きの力を加える
            Animator.SetBool("IsJumping", false);
            Animator.applyRootMotion = true;
        }
    }

    /// <summary>
    /// 落下中かどうかの判定を行う
    /// </summary>
    private void HandleFalling()
    {
        Animator.SetBool("IsGround", _playerBlackBoard.IsGrounded);
        /*
        //接地判定はfalseだが、落下中と判定しない例外
        //ジャンプ中/壁登り中/乗り越え中
        if (!_playerState.IsJumping && !_playerState.IsClimbing && !_playerState.IsVaulting)
        {
            _animator.SetBool("IsGround", !_playerState.IsGrounded);
        }
        */
    }

    public Vector3 TargetPosition => _collider.ClosestPoint(_targetTransform.position);
    
    /// <summary>アニメーションイベントでSEを再生するためのメソッド</summary>
    public void PlaySE(int index) => AudioManager.Instance?.PlaySE(index);
}