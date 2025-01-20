using System.Collections;
using System.Collections.Generic;
using PlayerSystem.Fight;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// エネミーの中心となるクラス
/// </summary>
[RequireComponent(typeof(NavMeshAgent), typeof(EnemyHealth), typeof(EnemyCombat))]
public class EnemyBrain : MonoBehaviour
{
    [SerializeField][HighlightIfNull] private Transform _player; //プレイヤーの参照
    
    //ステート管理
    private enum State{ Idle, Chase, Attack, Dead }
    private State _currentState = State.Idle;
    
    //コンポーネント
    private NavMeshAgent _agent;
    private ICombat _combat;
    private IHealth _health;
        
    [Header("パラメーター")]
    [SerializeField, Comment("プレイヤーを発見できる距離")] private float _detectionRange = 10f;
    [SerializeField, Comment("攻撃を開始する距離")] private float _attackRange = 2f;
    [SerializeField, Comment("攻撃間隔")] private float _attackCooldown = 1.5f;
    
    private float _attackTimer;

    private void Awake()
    {
        //コンポーネントを取得する
        _agent = GetComponent<NavMeshAgent>();
        _combat = GetComponent<ICombat>();
        _health = GetComponent<IHealth>();

        Debug.Log("呼ばれてる");
    }

    private void Update()
    {
        if(_health.IsDead)
        {
            TransitionToState(State.Dead);
            return; //死亡していたらこれ以降の処理を行わない
        }
        
        ChackState();
    }

    /// <summary>
    /// 現在のステートに合わせて行動を行うメソッドを呼び出す
    /// </summary>
    private void ChackState()
    {
        switch (_currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Chase:
                HandleChaseState();
                break;
            case State.Attack:
                HandleAttackState();
                break;
            case State.Dead:
                HandleDeadState();
                break;
        }
    }

    /// <summary>
    /// 現在のステートを変更する
    /// </summary>
    private void TransitionToState(State newState)
    {
        if(_currentState == newState) return; //ステートが変わらない場合これ以降の処理を行わない
        _currentState = newState;
    }

    /// <summary>
    /// Idle状態の処理
    /// </summary>
    private void HandleIdleState()
    {
        //プレイヤーとの距離が、発見できる距離より短かったら追跡状態に移行する
        if (Vector3.Distance(_player.position, transform.position) <= _detectionRange)
        {
            TransitionToState(State.Chase);
        }
    }

    /// <summary>
    /// 追跡状態の処理
    /// </summary>
    private void HandleChaseState()
    {
        _agent.SetDestination(_player.position); //移動先の目的地点を更新する

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        
        if (distanceToPlayer <= _attackRange)
        {
            TransitionToState(State.Attack); //攻撃範囲に到達したら攻撃状態に移行する
        }
        else if (distanceToPlayer > _detectionRange)
        {
            TransitionToState(State.Idle); //発見できる距離より離れたらidle状態に移行
        }
    }

    /// <summary>
    /// 攻撃状態の処理
    /// </summary>
    private void HandleAttackState()
    {
        _agent.SetDestination(transform.position); //現在の位置で停止

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        
        if (distanceToPlayer > _attackRange)
        {
            TransitionToState(State.Chase); //攻撃できる距離より離れたら追跡状態に移行する
            return;
        }

        // 攻撃処理
        _attackTimer -= Time.deltaTime; //クールタイムをTime.deltaTimeごとに減らしていくような計算方法
        if (_attackTimer <= 0f)
        {
            _combat.Attack();
            _attackTimer = _attackCooldown;
        }
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
}
