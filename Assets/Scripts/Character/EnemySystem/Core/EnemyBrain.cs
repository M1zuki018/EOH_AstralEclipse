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
    //コンポーネント
    private ICombat _combat;
    private EnemyMovement _enemyMovement;
    private Collider _collider;
    public Animator Animator { get; private set; }
    
    public Vector3 TargetPosition { get; }

    private void Start()
    {
        //コンポーネントを取得する
        _enemyMovement = GetComponent<EnemyMovement>();
        _combat = GetComponent<EnemyCombat>();
        _collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
        
        UIManager.Instance.RegisterEnemy(this, GetCurrentHP()); //HPバーを頭上に生成する
        UIManager.Instance.HideEnemyHP(this); //隠す
        
        //ターゲットマッチング用
        MatchPositionSMB smb = Animator.GetBehaviour<MatchPositionSMB>();
        smb._target = this;
    }
    
    protected override void HandleDamage(int damage, GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}から{damage}ダメージ受けた！ 現在{GetCurrentHP()})");
        UIManager.Instance.ShowDamageAmount(damage);
        UIManager.Instance.UpdateEnemyHP(this, GetCurrentHP()); //HPスライダーを更新する
        Animator.SetTrigger("Damage");
    }

    protected override void HandleDeath(GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}に倒された！");
        UIManager.Instance.UnregisterEnemy(this); //HPスライダーを削除する
        Animator.SetTrigger("Dead");
        //TODO:死亡エフェクト等の処理
        
        //コンポーネントの無効化
        _enemyMovement.enabled = false;
    }
}
