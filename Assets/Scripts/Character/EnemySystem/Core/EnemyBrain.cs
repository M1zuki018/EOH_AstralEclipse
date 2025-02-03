using PlayerSystem.Fight;
using UnityEngine;

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
    [SerializeField] private LockOnFunction _lockOnFunction; //プレイヤーのLockOn機能
    [SerializeField] private ReadyForBattleChecker _readyForBattleChecker;
    [SerializeField, Highlight(1,0,0.2f)] private bool _isBossEnemy = false;
    
    public Vector3 TargetPosition { get; }
    public IHealth Health => _health;
    public EnemyMovement EnemyMovement { get; private set; }
    public bool IsBossEnemy => _isBossEnemy;

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
        if (!_isBossEnemy)
        {
            //通常の敵の処理
            UIManager.Instance?.RegisterEnemy(this, GetCurrentHP()); //HPバーを頭上に生成する
            UIManager.Instance?.HideEnemyHP(this); //隠す
        }
        else
        {
            //ボス用の処理
            UIManager.Instance?.InitializeBossHP(_health.MaxHP, _health.CurrentHP); //HPバーを初期化
            UIManager.Instance?.UpdateBossName("Unknown"); //名前更新
            UIManager.Instance?.UpdateRemainingHP(Mathf.RoundToInt(GetCurrentHP() / _health.MaxHP * 100)); //HPパーセントの表記を更新
            UIManager.Instance?.HideBossUI();
        }

        //ターゲットマッチング用
        if (Animator.GetBehaviour<MatchPositionSMB>() != null)
        {
            MatchPositionSMB smb = Animator.GetBehaviour<MatchPositionSMB>();
            smb._target = this;
        }
    }
    
    protected override void HandleDamage(int damage, GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}から{damage}ダメージ受けた！ 現在{GetCurrentHP()})");
        
        if (!_isBossEnemy) //通常の敵
        {
            //プレイヤーの探知範囲内の時のみ行う
            if(_readyForBattleChecker.EnemiesInRange.Contains(this))
            {
                UIManager.Instance?.ShowDamageAmount(damage, transform);
                UIManager.Instance?.UpdateEnemyHP(this, GetCurrentHP()); //HPスライダーを更新する
                AudioManager.Instance?.PlayVoice(1); //ダメージの声
            }
        }
        else //ボス
        {
            UIManager.Instance?.UpdateBossHP(GetCurrentHP()); //スライダー更新
            UIManager.Instance?.UpdateRemainingHP(Mathf.RoundToInt((float)GetCurrentHP() / _health.MaxHP * 100)); //パーセント表記更新
        }
        
        Animator.SetTrigger("Damage");
    }

    protected override void HandleDeath(GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}に倒された！");

        if (!_isBossEnemy) //通常の敵
        {
            _readyForBattleChecker.RemoveEnemy(this);
            UIManager.Instance?.UnregisterEnemy(this); //HPスライダーを削除する
            gameObject.tag = "Untagged";
            Animator.SetTrigger("Dead");
            _lockOnFunction.LockOn();
            //TODO:死亡エフェクト等の処理
        
            //コンポーネントの無効化
            EnemyMovement.enabled = false;
        }
        else //ボス
        {
            GameManager.Instance.SetGameState(GameState.Clear); //ゲームクリアにする
        }
    }

    #region デバッグ用メソッド

    [ContextMenu("Debug_EnemyDeath")]
    public void Debug_EnemyDeath()
    {
        _health.TakeDamage(GetCurrentHP(), GameObject.FindWithTag("Player"));
    }

    #endregion
    
}
