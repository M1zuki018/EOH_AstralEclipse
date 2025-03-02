using UnityEngine;
using PlayerSystem.ActionFunction;
using PlayerSystem.Animation;
using PlayerSystem.Fight;
using PlayerSystem.Movement;
using PlayerSystem.State;

/// <summary>
/// プレイヤーの移動・空中移動などの処理を行う機能
/// ※PlayerBrainで黒板が生成されるまで動かない
/// </summary>
public class PlayerController : MonoBehaviour, IMatchTarget
{ 
    [Header("コンポーネント")]
    [SerializeField][ReadOnlyOnRuntime] private Transform _playerCamera; // カメラ
    [SerializeField][ReadOnlyOnRuntime] private CharacterController _cc;
    [SerializeField][ReadOnlyOnRuntime] private Animator _animator;
    
    [SerializeField] private Transform _targetTransform;
    
    private Collider _collider;
    
    #region 各種機能
    private PlayerBlackBoard _bb;
    
    private IMovable _mover; //移動
    private IJumpable _jumper; //ジャンプ
    private ISpeedSwitchable _speedSwitcher; //歩きと走り状態の切り替え
    private ISteppable _stepFunction; //ステップ
    private PlayerCombat _combat;
    
    private PlayerGravity _playerGravity; // 重力をかける処理
    private MovementHelper _movementHelper; // 移動処理を補助するクラス
    private IHandleGroundedCheck _handleGrounded; // 地面にいるときの処理を行うクラス
    private PlayerAnimationController _animController; // アニメーションを制御するクラス
    private GroundTrigger _groundTrigger; // 接地判定を管理するクラス
    private FootprintEffect _footprintEffect; // 足跡エフェクトを管理するクラス
    
    private PlayerActionHandler _playerActionHandler; // ステートマシンと処理をつなぐ
    public PlayerActionHandler PlayerActionHandler => _playerActionHandler;
    #endregion
    
    private void Awake()
    {
        InitializeComponents(); // 各コンポ―ネントを初期化
        SetupAnimator(); // アニメーションの設定を行う

        _speedSwitcher.Walk(); // 移動速度切り替えのObservableを購読する
    }

    #region Awakeの中の処理
    private void InitializeComponents()
    {
        _bb = GetComponent<PlayerBrain>().BB;
        
        _combat = GetComponent<PlayerCombat>();
        _footprintEffect = GetComponent<FootprintEffect>();
        TryGetComponent(out _collider);
        
        // インスタンスを生成
        _animController = new PlayerAnimationController(_bb, _animator);
        _movementHelper = new MovementHelper(_playerCamera, _bb, _cc);
        _mover = new PlayerMovementFunction(_bb, _animController, _movementHelper);
        _jumper = new PlayerJumpFunction(_bb, _cc, _animController,_movementHelper);
        _speedSwitcher = new PlayerSpeedSwitchFunction(_bb);
        _groundTrigger = new GroundTrigger(_bb, transform);
        _playerGravity = new PlayerGravity(_bb, _cc);
        _handleGrounded = (IHandleGroundedCheck) new HandleGrounded(_bb, _animController);
        
        _playerActionHandler = new PlayerActionHandler(
            mover: _mover,
            jumper: _jumper,
            steppable: new StepFunction(_animController, _bb),
            gauder: new GuardFunction(_bb),
            attack: (IAttack) _combat,
            skill: new SkillFunction(_bb),
            counter: new CounterFunction(_bb));
        
        _bb.AnimController = _animController;
    }
    
    /// <summary>
    /// Animatorの設定を行う
    /// </summary>
    private void SetupAnimator()
    {
        _animator.keepAnimatorStateOnDisable = true;
        _animator.applyRootMotion = true; //ルートモーションを有効化

        // ターゲットマッチングを行うStateMachineBehaviorに自身を登録する
        foreach (var smb in _animator.GetBehaviours<MatchPositionSMB>())
        {
            smb._target = this;
        }
    }
    #endregion
    
    private void FixedUpdate()
    {
        _playerGravity.ApplyGravity();
        _groundTrigger.CheckGrounded(); // 接地判定
        
        if (_bb.IsJumping)
        {
            _jumper.Jumping(); //ジャンプ処理
            //落下中の判定
        }
        else
        {
            _mover.Move(); //移動処理
            //落下中の判定
        }

        _animController.Movement.PlayFallingAnimation(_bb.IsGrounded); //落下中の判定
        _handleGrounded.HandleGroundedCheck();
    }
    
    public Vector3 TargetPosition => _collider.ClosestPoint(_targetTransform.position);

    /// <summary>アニメーションイベントでSEを再生するためのメソッド</summary>
    public void PlaySE(int index)
    {
        AudioManager.Instance?.PlaySE(index);
        _footprintEffect.CreateFootprint();
    }
}