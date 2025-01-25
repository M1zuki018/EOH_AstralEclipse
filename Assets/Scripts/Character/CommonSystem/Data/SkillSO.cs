using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Create SkillSO")]
public class SkillSO : ScriptableObject
{
    [SerializeField] private List<SkillData> _skillSet = new List<SkillData>();
    
    /// <summary>スキル発動時のイベント</summary>
    public event Action OnCastEvent;

    /// <summary>
    /// スキルを発動するときに呼ぶ
    /// </summary>
    public void Cast(int index)
    {
        /*
        //条件が満たされていない場合は発動しない
        if(!_skillSet[index].CastCondition.IsSatisfied())
        {
            Debug.Log($"{_skillSet[index].Name} の発動条件が満たされていません");
            return;
        }
        */
        
        OnCastEvent?.Invoke(); //スキル発動のイベントを発火する
        Debug.Log("スキル発動");
    }
}
