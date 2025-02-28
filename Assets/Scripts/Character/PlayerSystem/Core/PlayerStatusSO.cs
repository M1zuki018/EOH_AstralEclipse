using UnityEngine;

/// <summary>
/// プレイヤーのステータス情報を管理するスクリプト
/// </summary>
[CreateAssetMenu(fileName = "PlayerStatus", menuName = "GameData/PlayerStatus")]
public class PlayerStatusSO : ScriptableObject
{
    [Header("ヘルス系")]
    [SerializeField][Comment("最大HP")] private int _maxHP = 300;
    [SerializeField][Comment("ウィル")] private int _will = 150;
    
    [Header("戦闘関連")]
    [SerializeField][Comment("最大TP")] private int _maxTP = 100;
    [SerializeField][Comment("基礎攻撃力")] private int _baseAttackPower = 10;
    [SerializeField][Comment("防御力")] private int _defense = 0;
    [SerializeField][Comment("スキルSO")] private SkillSO _skill = null;
    [SerializeField][Comment("パリィ受付時間")] private int _parryReceptionTime = 200;
    [SerializeField][Comment("カウンター時間")] private float _counterTime = 3;
    
    public int MaxHP => _maxHP;
    public int Will => _will;
    
    public int MaxTP => _maxTP;
    public int BaseAttackPower => _baseAttackPower;
    public int Defense => _defense;
    public int ParryReceptionTime => _parryReceptionTime;
    public float CounterTime => _counterTime;
    
    /// <summary>IDで指定したスキルデータを返す</summary>
    public SkillData SkillData (int skillID) => _skill.Cast(skillID);
}
