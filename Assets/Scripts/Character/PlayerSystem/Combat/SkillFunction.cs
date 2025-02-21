using PlayerSystem.State;
using UnityEngine;

namespace PlayerSystem.Fight
{
    /// <summary>
    /// スキル発動に関する機能をまとめたクラス
    /// </summary>
    public class SkillFunction :ISkill
    {
        private PlayerBlackBoard _bb;

        public SkillFunction(PlayerBlackBoard bb)
        {
            _bb = bb;
        }

        /// <summary>
        /// 現在のTPと使用スキルのコストを比較して、スキルが使えるか判定する
        /// </summary>
        public bool CheckTP()
        {
            return true;
        }

        /// <summary>
        /// スキルを使う
        /// </summary>
        public void UseSkill()
        {
            SkillData skill = _bb.Status.SkillData(_bb.UsingSkillIndex); //スキルデータを取得する
            UIManager.Instance.SelectedSkillIcon(_bb.UsingSkillIndex);

            if (_bb.CurrentTP < skill.ResourceCost) //TPの判定を行う
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
        
            _bb.CurrentTP -= skill.ResourceCost; //TPを減らす
        
            UIManager.Instance?.UpdatePlayerTP(_bb.CurrentTP);
            Debug.Log($"スキルを使った　発動：{skill.Name}");
        }
    }
}

