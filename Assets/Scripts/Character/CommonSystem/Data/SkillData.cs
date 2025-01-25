using System;
using UnityEngine;

/// <summary>
/// スキルデータの構造体
/// </summary>
[Serializable]
public struct SkillData
{
    public int ID; //ID
    public string Name; //スキル名
    public string Descriptionl; //スキルの説明
    public Sprite Icon; // スキルアイコン
    public float AttackMultiplier; // 攻撃倍率
    public float Cooldown; // クールタイム（秒）
    public int ResourceCost; // 消費リソース量
    public SkillType Type; // スキルの種類
    public float CastTime; // 発動時間
    public float Range; // スキルの効果範囲
    public GameObject EffectPrefab; // エフェクトのPrefab
    public AnimationClip AnimationClip; // 発動時のアニメーション //TODO:これでいいか？
    public SkillCondition CastCondition; // 発動条件（オプション）
}
