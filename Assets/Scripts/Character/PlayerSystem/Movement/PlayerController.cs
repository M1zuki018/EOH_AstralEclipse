using UnityEngine;
using Cinemachine;
using PlayerSystem.ActionFunction;
using PlayerSystem.Input;
using PlayerSystem.Movement;
using PlayerSystem.State;
using UnityEngine.Serialization;

/// <summary>
/// プレイヤーの移動機能
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
    
    // 入力情報
    private IPlayerInputReceiver _playerInputReceiver;
    public IPlayerInputReceiver PlayerInputReceiver => _playerInputReceiver;
    
    // プレイヤーの状態
    private PlayerState _playerState;
    public PlayerState PlayerState => _playerState;

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
        _playerState = new PlayerState();
    }
    
    private void InitializeComponents()
    {
        //インスタンスを生成
        _mover = new PlayerMover(_characterController, Animator, _playerState, _playerCamera, GetComponent<TrailRenderer>());
        _jumper = (IJumpable) _mover;
        _walker = (IWalkable) _mover;
        
        // 入力情報のインスタンスを生成
        _playerInputReceiver = new PlayerInputProcessor(_playerState, _mover, _jumper, _walker, 
            GetComponent<StepFunction>(), GetComponent<GaudeFunction>(), GetComponent<LockOnFunction>(),
            GetComponent<PlayerCombat>());
        
        Animator.applyRootMotion = true; //ルートモーションを有効化
    }
    
    private void FixedUpdate()
    {
        if (_playerState.IsJumping)
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
        if (_playerState.IsGrounded && _playerState.Velocity.y < 0)
        {
            _playerState.IsJumping = false;
            _playerState.Velocity = new Vector3(0, -0.1f, 0); //確実に地面につくように少し下向きの力を加える
            Animator.SetBool("IsJumping", false);
            Animator.applyRootMotion = true;
        }
    }

    /// <summary>
    /// 落下中かどうかの判定を行う
    /// </summary>
    private void HandleFalling()
    {
        Animator.SetBool("IsGround", _playerState.IsGrounded);
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