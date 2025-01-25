using System;
using UnityEngine;

/// <summary>
/// スキルの発動条件のデータ
/// </summary>
[Serializable]
public class SkillCondition : MonoBehaviour
{
    public Func<bool> IsSatisfied;

    public SkillCondition(Func<bool> condition)
    {
        IsSatisfied = condition;
    }
}
