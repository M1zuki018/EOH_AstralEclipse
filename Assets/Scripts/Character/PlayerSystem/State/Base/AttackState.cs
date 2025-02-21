using System;
using Cysharp.Threading.Tasks;
using PlayerSystem.State.Attack;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 攻撃状態
    /// </summary>
    public class AttackState : PlayerBaseState<BaseStateEnum>
    {
        private PlayerAttackSubStateMachine _attackSubSM;

        public AttackState(IPlayerStateMachine stateMachine, PlayerAttackSubStateMachine attackSubSM) 
            : base(stateMachine)
        {
            _attackSubSM = attackSubSM;
        }
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            Debug.Log("AttackState : Enter");
            
            _attackSubSM.ChangeState(AttackStateEnum.NormalAttack1);
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                
            StateMachine.ChangeState(BaseStateEnum.Idle);
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            await UniTask.Yield();
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            _attackSubSM.ChangeState(AttackStateEnum.Default); // Defaultにして処理が何もない状態にしておく
            
            await UniTask.Yield();
        }
    }

}