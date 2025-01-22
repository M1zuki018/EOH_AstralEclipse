using PlayerSystem.Fight;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// エネミーの中心となるクラス
/// </summary>
[RequireComponent(typeof(EnemyMovement),typeof(Health),typeof(EnemyCombat))]
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class EnemyBrain : CharacterBase, IMatchTarget
{
    [SerializeField] private int _maxHP = 100;
    private int _currentHP;
    
    //コンポーネント
    private ICombat _combat;
    private EnemyMovement _enemyMovement;
    private Collider _collider;
    public Animator Animator { get; private set; }

    private void Start()
    {
        //コンポーネントを取得する

        _enemyMovement = GetComponent<EnemyMovement>();
        _combat = GetComponent<EnemyCombat>();
        _collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();

        _health.OnDamaged += HandleDamage; //ダメージ時イベント登録
        _health.OnDeath += HandleDeath; //死亡時イベント登録
        
        _uiManager.RegisterEnemy(this, _maxHP);

        //ターゲットマッチング用
        MatchPositionSMB smb = Animator.GetBehaviour<MatchPositionSMB>();
        smb._target = this;
    }

    private void Update()
    {
        Animator.SetInteger("HP", _currentHP);
        Animator.SetFloat("Speed", _enemyMovement.Velocity.magnitude);
    }

    private void OnDestroy()
    {
        _health.OnDamaged -= HandleDamage; //ダメージ時イベント解除
        _health.OnDeath -= HandleDeath; //死亡時イベント解除
    }


    public Vector3 TargetPosition { get; }
    
    protected override void HandleDamage(int damage, GameObject attacker)
    {
        _uiManager.UpdateEnemyHP(this, GetCurrentHP()); //HPスライダーを更新する
    }

    protected override void HandleDeath(GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}に倒された！");
        //TODO:死亡エフェクト等の処理
        //Destroy(gameObject, 1.0f);
        // 死亡時の処理（アニメーション、消滅など）
    }
}
