using PlayerSystem.Fight;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

/// <summary>
/// エネミーの中心となるクラス
/// </summary>
[RequireComponent(typeof(EnemyMovement),typeof(Health),typeof(EnemyCombat))]
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class EnemyBrain : CharacterBase, IMatchTarget
{
    //コンポーネント
    private ICombat _combat;
    private Collider _collider;
    public Animator Animator { get; private set; }
    [SerializeField] private LockOnFunction _lockOnFunction;
    [SerializeField] private ReadyForBattleChecker _readyForBattleChecker;
    
    public Vector3 TargetPosition { get; }
    public IHealth Health
    {
        get { return _health; }
    }
    public EnemyMovement EnemyMovement { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        //コンポーネントを取得する Start関数より前に実行したい
        EnemyMovement = GetComponent<EnemyMovement>();
        _combat = GetComponent<EnemyCombat>();
        _collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
    }
    
    private void Start() 
    {
        UIManager.Instance.RegisterEnemy(this, GetCurrentHP()); //HPバーを頭上に生成する
        UIManager.Instance.HideEnemyHP(this); //隠す
        
        //ターゲットマッチング用
        MatchPositionSMB smb = Animator.GetBehaviour<MatchPositionSMB>();
        smb._target = this;
    }
    
    protected override void HandleDamage(int damage, GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}から{damage}ダメージ受けた！ 現在{GetCurrentHP()})");
        UIManager.Instance.ShowDamageAmount(damage, transform);
        UIManager.Instance.UpdateEnemyHP(this, GetCurrentHP()); //HPスライダーを更新する
        AudioManager.Instance.PlayVoice(1); //ダメージの声
        Animator.SetTrigger("Damage");
    }

    protected override void HandleDeath(GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}に倒された！");
        _readyForBattleChecker.RemoveEnemy(this);
        UIManager.Instance.UnregisterEnemy(this); //HPスライダーを削除する
        gameObject.tag = "Untagged";
        Animator.SetTrigger("Dead");
        _lockOnFunction.LockOn();
        //TODO:死亡エフェクト等の処理
        
        //コンポーネントの無効化
        EnemyMovement.enabled = false;
    }

    #region デバッグ用メソッド

    [ContextMenu("Debug_EnemyDeath")]
    public void Debug_EnemyDeath()
    {
        _health.TakeDamage(GetCurrentHP(), gameObject);
    }

    #endregion
    
}
