using System.Collections;
using System.Collections.Generic;
using Enemy.State;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// エネミーの移動クラス（NavMeshの制御）
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] [HighlightIfNull] private Transform _target; //プレイヤーの参照
    private EnemyState _currentState = EnemyState.Idle; //状態

    public Vector3 Velocity { get; private set; }

    [Header("パラメーター")] [SerializeField, Comment("プレイヤーを発見できる距離")]
    private float _detectionRange = 10f;

    [SerializeField, Comment("攻撃を開始する距離")] private float _attackRange = 2f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (_currentState == EnemyState.Dead)
        {
            //死亡状態なら動作停止。これ以降の処理は行わない
            _agent.isStopped = true;
            return;
        }

        if (_target == null) return; //ターゲットがいない場合、これ以降の処理は行わない
        
        ChackState(); //現在のステートをチェック
    }
    
    /// <summary>
    /// 現在のステートに合わせて行動を行うメソッドを呼び出す
    /// </summary>
    private void ChackState()
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
                HandleAttackState();
                break;
            case EnemyState.Dead:
                HandleDeadState();
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
        Velocity = Vector3.zero; //idle状態の時は、Speedはゼロに固定する
        
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
        _agent.SetDestination(_target.position); //移動先の目的地点を更新する
        
        Velocity = _agent.velocity.normalized; //Animator制御のため
        
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

    /// <summary>
    /// 攻撃状態の処理
    /// </summary>
    private void HandleAttackState()
    {
        _agent.SetDestination(transform.position); //現在の位置で停止

        float distanceToPlayer = Vector3.Distance(transform.position, _target.position);
        
        if (distanceToPlayer > _attackRange)
        {
            TransitionToState(EnemyState.Chase); //攻撃できる距離より離れたら追跡状態に移行する
            return;
        }

        /*
        // 攻撃処理
        _attackTimer -= Time.deltaTime; //クールタイムをTime.deltaTimeごとに減らしていくような計算方法
        if (_attackTimer <= 0f)
        {
            _combat.Attack(this);
            _attackTimer = _attackCooldown;
        }
        */
    }

    /// <summary>
    /// 死亡状態の処理
    /// </summary>
    private void HandleDeadState()
    {
        // 死亡時の処理（アニメーション、消滅など）
        Debug.Log($"{gameObject.name} を倒した");
        _agent.isStopped = true;
        Destroy(gameObject, 2f); // 2秒後にオブジェクトを破壊
    }

    public EnemyState GetState() => _currentState;

}
