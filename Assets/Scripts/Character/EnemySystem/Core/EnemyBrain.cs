using Enemy.State;
using PlayerSystem.Fight;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// エネミーの中心となるクラス
/// </summary>
[RequireComponent(typeof(NavMeshAgent),  typeof(EnemyCombat))]
public class EnemyBrain : CharacterBase, IMatchTarget
{
    [SerializeField][HighlightIfNull] private Transform _player; //プレイヤーの参照
    [SerializeField] private int _maxHP = 100;
    private int _currentHP;
    
    //ステート管理
    private EnemyState _currentState = EnemyState.Idle;
    
    //コンポーネント
    private NavMeshAgent _agent;
    private ICombat _combat;
    private Collider _collider;
    public Animator Animator { get; private set; }
        
    [Header("パラメーター")]
    [SerializeField, Comment("プレイヤーを発見できる距離")] private float _detectionRange = 10f;
    [SerializeField, Comment("攻撃を開始する距離")] private float _attackRange = 2f;
    [SerializeField, Comment("攻撃間隔")] private float _attackCooldown = 1.5f;
    
    private float _attackTimer;

    private void Awake()
    {
        //コンポーネントを取得する
        _agent = GetComponent<NavMeshAgent>();
        _combat = GetComponent<EnemyCombat>();
        _health = GetComponent<IHealth>();
        _collider = GetComponent<Collider>();
        _uiManager = GetComponent<UIManager>();
        Animator = GetComponent<Animator>();

        _health.OnDamaged += HandleDamage; //ダメージ時イベント登録
        _health.OnDeath += HandleDeath; //死亡時イベント登録

        //ターゲットマッチング用
        MatchPositionSMB smb = Animator.GetBehaviour<MatchPositionSMB>();
        smb._target = this;
    }

    private void Update()
    {
        if(_health.IsDead)
        {
            TransitionToState(EnemyState.Dead);
            return; //死亡していたらこれ以降の処理を行わない
        }
        
        Animator.SetInteger("HP", _currentHP);
        ChackState();
    }

    private void OnDestroy()
    {
        _health.OnDamaged -= HandleDamage; //ダメージ時イベント解除
        _health.OnDeath -= HandleDeath; //死亡時イベント解除
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
        //プレイヤーとの距離が、発見できる距離より短かったら追跡状態に移行する
        if (Vector3.Distance(_player.position, transform.position) <= _detectionRange)
        {
            TransitionToState(EnemyState.Chase);
        }
        Animator.SetFloat("Speed", 0); //idle状態の時は、Speedはゼロに固定する
    }

    /// <summary>
    /// 追跡状態の処理
    /// </summary>
    private void HandleChaseState()
    {
        _agent.SetDestination(_player.position); //移動先の目的地点を更新する

        //Animatorを制御する
        Vector3 velocity = _agent.velocity.normalized;
        Animator.SetFloat("Speed", velocity.magnitude, 0.1f, Time.deltaTime);
        
        //プレイヤーと自身の距離をはかる
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        
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

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        
        if (distanceToPlayer > _attackRange)
        {
            TransitionToState(EnemyState.Chase); //攻撃できる距離より離れたら追跡状態に移行する
            return;
        }

        // 攻撃処理
        _attackTimer -= Time.deltaTime; //クールタイムをTime.deltaTimeごとに減らしていくような計算方法
        if (_attackTimer <= 0f)
        {
            _combat.Attack(this);
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
    
    public Vector3 TargetPosition { get; }
    
    protected override void HandleDamage(int damage, GameObject attacker)
    {
        _uiManager.UpdateEnemyHP(damage, 0);
        //TODO:エネミーHPバーの管理方法を考える
    }

    protected override void HandleDeath(GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}に倒された！");
        //TODO:死亡エフェクト等の処理
        //Destroy(gameObject, 1.0f);
    }
}
