using UnityEngine;
using PlayerSystem.ActionFunction;
using PlayerSystem.Fight;
using PlayerSystem.Movement;

/// <summary>
/// プレイヤーの移動・空中移動などの処理を行う機能
/// </summary>
public class PlayerController : MonoBehaviour, IMatchTarget
{ 
    [Header("コンポーネント")]
    [SerializeField][ReadOnlyOnRuntime] private Transform _playerTransform; // プレイヤーのTransform
    [SerializeField][ReadOnlyOnRuntime] private Transform _playerCamera; // カメラ
    [SerializeField][ReadOnlyOnRuntime] private CharacterController _cc;
    [SerializeField][ReadOnlyOnRuntime] private PlayerBrain _brain;
    
    // Animator
    [SerializeField][ReadOnlyOnRuntime] private Animator _animator;
    public Animator Animator => _animator;
    
    
    
    private Collider _collider;
    [SerializeField] private Transform _targetTransform;
    
    #region 各種機能
    private IMovable _mover; //移動
    private IJumpable _jumper; //ジャンプ
    private ISpeedSwitchable _speedSwitcher; //歩きと走り状態の切り替え
    private ISteppable _stepFunction; //ステップ
    
    private PlayerGravity _playerGravity; // 重力をかける処理
    private MovementHelper _movementHelper; // 移動処理を補助するクラス
    private IHandleGroundedCheck _handleGrounded; // 地面にいるときの処理を行うクラス
    private PlayerAnimationController _animationController; // アニメーションを制御するクラス
    private PlayerTrailController _trailController; // トレイルを管理するクラス
    
    private PlayerActionHandler _playerActionHandler; // ステートマシンと処理をつなぐ
    public PlayerActionHandler PlayerActionHandler => _playerActionHandler;
    #endregion
    
    private void Awake()
    {
        InitializeComponents();
        
        TryGetComponent(out _collider);
        Animator.keepAnimatorStateOnDisable = true;
        
        // ターゲットマッチングを行うStateMachineBehaviorに自身を登録する
        foreach (var smb in Animator.GetBehaviours<MatchPositionSMB>())
        {
            smb._target = this;
        }
        
        _speedSwitcher.Walk(); // 移動速度切り替えのObservableを購読する
    }
    
    private void InitializeComponents()
    {
        _movementHelper = new MovementHelper(_playerCamera, _brain.BB, _cc);
        _animationController = new PlayerAnimationController(_animator);
        _trailController = new PlayerTrailController(GetComponent<TrailRenderer>());
        
        // インスタンスを生成
        _mover = new PlayerMovementFunction(_brain.BB, _animationController, _trailController, _movementHelper);
        _jumper = new PlayerJumpFunction(_brain.BB, _cc, _animationController,_trailController, _movementHelper);
        _speedSwitcher = new PlayerSpeedSwitchFunction(_brain.BB);

        _playerActionHandler = new PlayerActionHandler(
            mover: _mover,
            jumper: _jumper,
            steppable: new StepFunction(Animator, _brain.BB),
            gauder: new GuardFunction(_brain.BB),
            attack: (IAttack) GetComponent<PlayerCombat>(),
            skill: new SkillFunction(_brain.BB));

        _playerGravity = new PlayerGravity(_brain.BB, _cc);
        _handleGrounded = (IHandleGroundedCheck) new HandleGrounded(_brain.BB, _animator);
        
        Animator.applyRootMotion = true; //ルートモーションを有効化
    }

    private void OnDestroy()
    {
        _speedSwitcher.DisposeWalkSubscription(); // 移動速度切り替えのObservableを購読解除
    }

    private void FixedUpdate()
    {
        _playerGravity.ApplyGravity();
        
        if (_brain.BB.IsJumping)
        {
            _jumper.Jumping(); //ジャンプ処理
            HandleFalling(); //落下中の判定
        }
        else
        {
            _mover.Move(); //移動処理
            HandleFalling(); //落下中の判定
        }
        
        _handleGrounded.HandleGroundedCheck();
    }

    /// <summary>
    /// 落下中かどうかの判定を行う
    /// </summary>
    private void HandleFalling()
    {
        Animator.SetBool("IsGround", _brain.BB.IsGrounded);
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