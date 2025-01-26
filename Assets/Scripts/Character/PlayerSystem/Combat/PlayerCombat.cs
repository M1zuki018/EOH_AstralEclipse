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
    public UIManager _uiManager;
    private ReadyForBattleChecker _battleChecker;
    [SerializeField] private SkillSO _skillSet;
    [SerializeField] private GameObject _weaponObj;
    public SkillSO SkillSet => _skillSet;
    private int _stage;
    
    private void Start()
    {
        //コンポーネントを取得する
        _playerMovement = GetComponent<PlayerMovement>();
        _damageHandler = new DamageHandler();
        Detector = GetComponentInChildren<AttackHitDetector>();
        _battleChecker = GetComponentInChildren<ReadyForBattleChecker>(); //子オブジェクトから取得
        
        _weaponObj.SetActive(false);
        
        _uiManager.InitializePlayerTP(TP, TP); //TPゲージを初期化
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
    private void HandleReadyForBattle()
    {
        if (!_weaponObj.activeSelf) //まだ武器を構えていなかったら、以降の処理を行う
        {
            _playerMovement._animator.SetTrigger("ReadyForBattle");
            _weaponObj.SetActive(true); //武器のオブジェクトを表示する
            AudioManager.Instance.PlaySE(2);
        }
    }
    
    /// <summary>
    /// 臨戦状態が解除されたときの処理。武器をしまう
    /// </summary>
    private void HandleRescission()
    {
        if (_weaponObj.activeSelf)
        {
            Debug.Log("臨戦状態解除");
            _weaponObj.SetActive(false);
            AudioManager.Instance.PlaySE(2);
        }
    }
    /// <summary>
    /// 攻撃入力を受けた時に呼び出される処理
    /// </summary>
    public void Attack()
    {
        _playerMovement._animator.SetTrigger("Attack"); //アニメーションのAttackをトリガーする
        Detector.CurrentStage = _stage;
        AudioManager.Instance.PlaySE(3); //TODO:まだ無限に音がなるので直す
        
        List<IDamageable> damageables = Detector.PerformAttack();
        foreach (IDamageable damageable in damageables)
        {
            _damageHandler.ApplyDamage(damageable, BaseAttackPower, 0, gameObject);
        }
    }

    /// <summary>
    /// アニメーションイベントで呼び出す関数
    /// 現在何段目の攻撃か取得できる
    /// </summary>
    public void OnAttackStage(int stage)
    {
        _stage = stage;
    }
    
    /*
    public async UniTaskVoid PerformAttack()
    {
        float attackDuration = 1.0f; // 攻撃の持続時間を設定（例: 1秒）
        await Detector.StartAttack(attackDuration);
    }
    */

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
        
        _uiManager.UpdatePlayerTP(TP);
        Debug.Log($"スキルを使った　発動：{skill.Name}");
    }
}