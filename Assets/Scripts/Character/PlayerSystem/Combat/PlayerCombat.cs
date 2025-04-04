using Cysharp.Threading.Tasks;
using PlayerSystem.Fight;
using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// プレイヤーの攻撃に関する処理
/// </summary>
public class PlayerCombat : ViewBase, ICombat, IAttack
{
    public AttackHitDetector Detector { get; private set; } // 当たり判定
    private PlayerBlackBoard _bb;
    private ReadyForBattleChecker _battleChecker; // 臨戦状態を管理するクラス

    [SerializeField][ReadOnlyOnRuntime] private GameObject _weaponObj;
    [SerializeField] private SkillSO _skillSet;
    
    [Header("攻撃補正用")]
    [SerializeField, HighlightIfNull] private AdjustDirection _adjustDirection;
    
    public int BaseAttackPower { get; set; }
    
    private WeaponHandler _weaponHandler;
    private ThrowingWeapon _throwingWeapon;
    
    private DamageHandler _damageHandler; // ダメージを与える処理があるクラス
    public DamageHandler DamageHandler => _damageHandler;
    public AdjustDirection AdjustDirection => _adjustDirection; // 攻撃時にプレイヤーの向きをターゲットに合わせる
    
    public override UniTask OnStart()
    {
        InitializeComponents();
        
        UIManager.Instance?.HideLockOnUI();
        UIManager.Instance?.HidePlayerBattleUI();
        _battleChecker.OnReadyForBattle += HandleReadyForBattle; //イベント登録
        _battleChecker.OnRescission += HandleRescission;
        
        return base.OnStart();
    }

    private void OnDestroy()
    {
        _battleChecker.OnReadyForBattle -= HandleReadyForBattle; //解除
        _battleChecker.OnRescission -= HandleRescission;
    }

    /// <summary>
    /// コンポーネントを取得する
    /// </summary>
    private void InitializeComponents()
    {
        _bb = GetComponent<PlayerBrain>().BB;
        _damageHandler = new DamageHandler();
        Detector = GetComponentInChildren<AttackHitDetector>();
        _battleChecker = GetComponentInChildren<ReadyForBattleChecker>(); //子オブジェクトから
        _weaponHandler = new WeaponHandler(_bb, _weaponObj);
        _bb.WeaponHandler = _weaponHandler;
        _throwingWeapon = new ThrowingWeapon(_bb, _weaponObj);
    }

    /// <summary>
    /// 武器を投げる
    /// </summary>
    public void ThrowWeapon() => _throwingWeapon.ThrowWeapon();

    /// <summary>
    /// 武器を投げる
    /// </summary>
    public void RecastTest() => _throwingWeapon.RecastWeapon();
    
    /// <summary>
    /// 臨戦状態になったときの処理。武器を取り出す
    /// </summary>
    private void HandleReadyForBattle(EnemyBrain brain)
    {
        if (!_bb.IsReadyArms)
        {
            _weaponHandler.HandleWeaponActivation();// まだ武器を構えていなかったら武器を構える処理を行う
        }

        //ボス戦の場合に行う処理
        if (_bb.IsBossBattle)
        {
            return;
        }
        
        //通常戦闘の場合に行う処理
        if (brain != null)
        {
            UIManager.Instance?.ShowEnemyHP(brain); //敵のHPバーを表示する（ボスはイベント側でHPバーを表示する）
        }
    }

    /// <summary>
    /// 臨戦状態が解除されたときの処理。武器をしまう
    /// </summary>
    private void HandleRescission(EnemyBrain brain)
    {
        if (_bb.IsBossBattle)
        {
            return; //ボス戦中は臨戦状態の解除を行わない。常に臨戦状態にする
        }

        //敵が死んでいない場合のみ処理を行う（死んでいる場合は処理が重複するので行わない）
        if (brain != null && !brain.gameObject.GetComponent<IHealth>().IsDead)
        {
            UIManager.Instance?.HideEnemyHP(brain); //敵のHPバーを非表示にする
            UIManager.Instance?.HideLockOnUI(); //ロックオンアイコンを非表示にする
        }

        if (_battleChecker.EnemiesInRange.Count == 0 && _bb.IsReadyArms)
        {
            _weaponHandler.HandleWeaponDeactivation();
        }
    }

    /// <summary>
    /// 攻撃入力を受けた時に呼び出される処理
    /// </summary>
    public void Attack()
    {
        if (!_bb.IsReadyArms)
        {
            //武器を構えてなかったら武器を構える
            _weaponHandler.HandleWeaponActivation();
        }

        _bb.AttackFinishedTrigger = false;
        _bb.IsAttacking = true; //解除はLocoMotionのSMBから行う
        _bb.AnimController.Combat.TriggerAttack();//アニメーションのAttackをトリガーする
    }
}