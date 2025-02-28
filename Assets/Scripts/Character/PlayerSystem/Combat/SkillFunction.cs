using System;
using Cysharp.Threading.Tasks;
using PlayerSystem.State;
using UnityEngine;

namespace PlayerSystem.Fight
{
    /// <summary>
    /// スキル発動に関する機能をまとめたクラス
    /// </summary>
    public class SkillFunction : ISkill
    {
        private PlayerBlackBoard _bb;
        private PlayerCombat _combat;
        private SkillData _skill;

        public SkillFunction(PlayerBlackBoard bb, PlayerCombat combat)
        {
            _bb = bb;
            _combat = combat;
            _bb.CurrentTP = _bb.Status.MaxTP;
            UIManager.Instance?.UpdatePlayerTP(_bb.Status.MaxTP); //TPゲージを初期化
        }

        /// <summary>
        /// 現在のTPと使用スキルのコストを比較して、スキルが使えるか判定する
        /// </summary>
        public bool CanUseSkill()
        {
            ChangeSkillData(); // スキルデータを取得する
            
            if (_bb.CurrentTP < _skill.ResourceCost) //TPの判定を行う
            {
                Debug.Log($"{_skill.Name} の発動にTPが足りません");
            }
            
            return _bb.CurrentTP > _skill.ResourceCost;
        }

        /// <summary>
        /// スキルを使う
        /// </summary>
        public async UniTask UseSkill()
        {
            // CanUseSkill が true の場合そのまま使用処理が呼ばれるので、スキルデータは取得しなおさない

            if (!_bb.IsReadyArms)
            {
                // まだ武器を構えていなかったら構える
                _combat.HandleWeaponActivation();
            }
            
            UIManager.Instance.SelectedSkillIcon(_bb.UsingSkillIndex);
            
            _bb.AnimController.Combat.UseSkill();
            
            //発動条件がセットされているとき、条件が満たされていない場合は発動しない
            if(_skill.CastCondition != null && !_skill.CastCondition.IsSatisfied())
            {
                Debug.Log($"{_skill.Name} の発動条件が満たされていません");
                return;
            }
        
            _bb.CurrentTP -= _skill.ResourceCost; //TPを減らす
        
            UIManager.Instance?.UpdatePlayerTP(_bb.CurrentTP);
            Debug.Log($"スキルを使った　発動：{_skill.Name}");
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            
            UIManager.Instance.DeSelectedSkillIcon(_bb.UsingSkillIndex);
        }

        /// <summary>
        /// スキルデータを取得する
        /// </summary>
        private void ChangeSkillData()
        {
            _skill = _bb.Status.SkillData(_bb.UsingSkillIndex);
        }
    }
}

