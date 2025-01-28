using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// プレイヤーの攻撃に関する処理
/// </summary>
public class PlayerCombat : MonoBehaviour, ICombat
{
    public int BaseAttackPower { get; private set; } = 10; //攻撃力
    public int TP { get; private set; } = 100; //TP
    public AttackHitDetector Detector { get; private set; }
    private PlayerMovement _playerMovement;
    private DamageHandler _damageHandler;
    private ReadyForBattleChecker _battleChecker;
    [SerializeField] private SkillSO _skillSet;
    [SerializeField] private GameObject _weaponObj;
    
    [Header("攻撃補正用")]
    [SerializeField, HighlightIfNull] private AdjustDirection _adjustDirection;

    [SerializeField, HighlightIfNull] private NormalAttack_First _first;
    
    public AdjustDirection AdjustDirection => _adjustDirection;
    
    public SkillSO SkillSet => _skillSet;
    
    private void Start()
    {
        //コンポーネントを取得する
        _playerMovement = GetComponent<PlayerMovement>();
        _damageHandler = new DamageHandler();
        Detector = GetComponentInChildren<AttackHitDetector>();
        _battleChecker = GetComponentInChildren<ReadyForBattleChecker>(); //子オブジェクトから取得。臨戦状態の判定
        
        _weaponObj.SetActive(false);
        
        UIManager.Instance.HideLockOnUI();
        UIManager.Instance.HidePlayerBattleUI();
        UIManager.Instance.InitializePlayerTP(TP, TP); //TPゲージを初期化
        _battleChecker.OnReadyForBattle += HandleReadyForBattle; //イベント登録
        _battleChecker.OnRescission += HandleRescission;
    }

    private void OnDestroy()
    {
        _battleChecker.OnReadyForBattle -= HandleReadyForBattle; //解除
        _battleChecker.OnRescission -= HandleRescission;
    }

    /// <summary>
    /// 臨戦状態になったときの処理。武器を取り出す
    /// </summary>
    private void HandleReadyForBattle(EnemyBrain brain)
    {
        if (brain != null)
        {
            UIManager.Instance.ShowEnemyHP(brain); //敵のHPバーを表示する
        }
        
        if (!_weaponObj.activeSelf) //まだ武器を構えていなかったら、以降の処理を行う
        {
            _playerMovement._animator.SetTrigger("ReadyForBattle");
            _weaponObj.SetActive(true); //武器のオブジェクトを表示する
            AudioManager.Instance.PlaySE(2);
            UIManager.Instance.ShowPlayerBattleUI();
        }
    }
    
    /// <summary>
    /// 臨戦状態が解除されたときの処理。武器をしまう
    /// </summary>
    private void HandleRescission(EnemyBrain brain)
    {
        //敵が死んでいない場合のみ処理を行う（死んでいる場合は処理が重複するので行わない）
        if (brain != null && !brain.gameObject.GetComponent<IHealth>().IsDead)
        {
            UIManager.Instance.HideEnemyHP(brain); //敵のHPバーを非表示にする
            UIManager.Instance.HideLockOnUI(); //ロックオンアイコンを非表示にする
        }
        
        if (_battleChecker.EnemiesInRange.Count == 0 && _weaponObj.activeSelf)
        {
            _weaponObj.SetActive(false);
            AudioManager.Instance.PlaySE(2);
            UIManager.Instance.HidePlayerBattleUI();
        }
    }
    
    /// <summary>
    /// 攻撃入力を受けた時に呼び出される処理
    /// </summary>
    public void Attack()
    {
        if (_battleChecker.ReadyForBattle)
        {
            _playerMovement._animator.SetTrigger("Attack"); //アニメーションのAttackをトリガーする
        }
    }

    /// <summary>
    /// Animator側から呼び出される処理
    /// </summary>
    public void PerformAttack(int index)
    {
        //当たり判定制御
        Detector.CurrentStage = index;
        
        List<IDamageable> damageables = Detector.PerformAttack();
        foreach (IDamageable damageable in damageables)
        {
            _damageHandler.ApplyDamage(damageable, BaseAttackPower, 0, gameObject);
        }
        
        if(index == 0) _first.StartAttack(_adjustDirection.Target); //攻撃1段目の補正
        
        AudioManager.Instance.PlaySE(3);
    }

    /// <summary>
    /// スキル処理
    /// </summary>
    public void UseSkill(int index)
    {
        SkillData skill = _skillSet.Cast(index); //スキルデータを取得する

        if (TP < skill.ResourceCost) //TPの判定を行う
        {
            Debug.Log($"{skill.Name} の発動にTPが足りません");
            return;
        }

        //発動条件がセットされているとき、条件が満たされていない場合は発動しない
        if(skill.CastCondition != null && !skill.CastCondition.IsSatisfied())
        {
            Debug.Log($"{skill.Name} の発動条件が満たされていません");
            return;
        }
        
        //ダメージを与える処理
        List<IDamageable> damageables = Detector.PerformAttack();
        foreach (IDamageable damageable in damageables)
        {
            _damageHandler.ApplyDamage(damageable, 
                baseDamage: Mathf.FloorToInt(BaseAttackPower * skill.AttackMultiplier), //攻撃力*スキル倍率。小数点以下切り捨て
                 0, gameObject);
        }
        
        TP -= skill.ResourceCost; //TPを減らす
        
        UIManager.Instance.UpdatePlayerTP(TP);
        Debug.Log($"スキルを使った　発動：{skill.Name}");
    }
}