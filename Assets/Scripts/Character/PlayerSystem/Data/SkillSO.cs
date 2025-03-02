using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Create SkillSO")]
public class SkillSO : ScriptableObject
{
    [SerializeField] private List<SkillData> _skillSet = new List<SkillData>();

    /// <summary>
    /// スキルデータを返す
    /// </summary>
    public SkillData Cast(int index)
    {
        return _skillSet[index - 1]; //入力は1オリジンで渡されるので、0オリジンに変換
    }
}
