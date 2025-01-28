using Enemy.State;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// エネミーの移動クラス（NavMeshの制御）
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }
    [SerializeField] private Transform _target; //プレイヤーの参照
    private EnemyState _currentState = EnemyState.Idle; //状態
    private EnemyBrain _brain;
    private EnemyCombat _combat; //戦闘クラスを参照
    private ReadyForBattleChecker _readyForBattleChecker; //臨戦態勢を管理
    private EnemyAI _enemyAI; //巡回中の動き
    private bool _playEncounteVoice = false;
    
    public Vector3 Velocity { get; private set; }

    [Header("パラメーター")]
    [SerializeField, Comment("プレイヤーを発見できる距離")] private float _detectionRange = 10f;
    [SerializeField, Comment("攻撃を開始する距離")] private float _attackRange = 2f;
    

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        _brain = GetComponent<EnemyBrain>();
        _combat = GetComponent<EnemyCombat>();
        _readyForBattleChecker = GetComponentInChildren<ReadyForBattleChecker>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _enemyAI = GetComponent<EnemyAI>();
    }

    private void Update()
    {
        if (_currentState == EnemyState.Dead)
        {
            //死亡状態なら動作停止。これ以降の処理は行わない
            Agent.isStopped = true;
            return;
        }

        if (_target == null) return; //ターゲットがいない場合、これ以降の処理は行わない
        
        CheckState(); //現在のステートをチェック
    }
    
    /// <summary>
    /// 現在のステートに合わせて行動を行うメソッドを呼び出す
    /// </summary>
    private void CheckState()
    {
        switch (_currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;
            case EnemyState.Chase:
                HandleChaseState();
                break;
            case EnemyState.Attack:
                _combat.HandleAttackState(_target, () => TransitionToState(EnemyState.Chase), _attackRange);
                break;
        }
    }
    
    /// <summary>
    /// 現在のステートを変更する
    /// </summary>
    private void TransitionToState(EnemyState newState)
    {
        if(_currentState == newState) return; //ステートが変わらない場合これ以降の処理を行わない
        _currentState = newState;
    }
    
    /// <summary>
    /// Idle状態の処理
    /// </summary>
    private void HandleIdleState()
    {
        _brain.Animator.SetFloat("Speed", 0.5f, 0.5f, Time.deltaTime);
        _enemyAI?.GoToNextPoint(); //巡回
        Velocity = Agent.velocity.normalized;
        //プレイヤーとの距離が、発見できる距離より短かったら追跡状態に移行する
        if (Vector3.Distance(_target.position, transform.position) <= _detectionRange)
        {
            TransitionToState(EnemyState.Chase);
        }
    }

    /// <summary>
    /// 追跡状態の処理
    /// </summary>
    private void HandleChaseState()
    {
        if (!_playEncounteVoice)
        {
            AudioManager.Instance.PlayVoice(0);
            _playEncounteVoice = true;
        }
        
        Agent.SetDestination(_target.position); //移動先の目的地点を更新する
        
        Velocity = Agent.velocity.normalized; //Animator制御のため
        _brain.Animator.SetFloat("Speed", 1f, 0.5f, Time.deltaTime);
        
        //プレイヤーと自身の距離をはかる
        float distanceToPlayer = Vector3.Distance(transform.position, _target.position);
        
        if (distanceToPlayer <= _attackRange)
        {
            TransitionToState(EnemyState.Attack); //攻撃範囲に到達したら攻撃状態に移行する
        }
        else if (distanceToPlayer > _detectionRange)
        {
            TransitionToState(EnemyState.Idle); //発見できる距離より離れたらidle状態に移行
        }
    }
    
    public EnemyState GetState() => _currentState;

}
