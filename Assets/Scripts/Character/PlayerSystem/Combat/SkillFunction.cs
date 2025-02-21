using System;
using Cysharp.Threading.Tasks;
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
            _bb.CurrentTP = _bb.Status.MaxTP;
            UIManager.Instance?.InitializePlayerTP(_bb.Status.MaxTP, _bb.Status.MaxTP); //TPゲージを初期化
        }

        /// <summary>
        /// 現在のTPと使用スキルのコストを比較して、スキルが使えるか判定する
        /// </summary>
        public bool CanUseSkill()
        {
            SkillData skill = _bb.Status.SkillData(_bb.UsingSkillIndex); //スキルデータを取得する
            if (_bb.CurrentTP < skill.ResourceCost) //TPの判定を行う
            {
                Debug.Log($"現在のTP {_bb.CurrentTP} / スキルコスト {skill.ResourceCost} / スキル番号 {_bb.UsingSkillIndex}");
                Debug.Log($"{skill.Name} の発動にTPが足りません");
            }
            
            return _bb.CurrentTP > skill.ResourceCost;
        }

        /// <summary>
        /// スキルを使う
        /// </summary>
        public async UniTask UseSkill()
        {
            SkillData skill = _bb.Status.SkillData(_bb.UsingSkillIndex); //スキルデータを取得する
            UIManager.Instance.SelectedSkillIcon(_bb.UsingSkillIndex);
            
            /*
            //発動条件がセットされているとき、条件が満たされていない場合は発動しない
            if(skill.CastCondition != null && !skill.CastCondition.IsSatisfied())
            {
                Debug.Log($"{skill.Name} の発動条件が満たされていません");
                return;
            }
            */
        
            _bb.CurrentTP -= skill.ResourceCost; //TPを減らす
        
            UIManager.Instance?.UpdatePlayerTP(_bb.CurrentTP);
            Debug.Log($"スキルを使った　発動：{skill.Name}");
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
    }
}

