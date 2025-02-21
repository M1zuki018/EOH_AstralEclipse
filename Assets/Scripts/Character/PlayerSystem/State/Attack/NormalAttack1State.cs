using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerSystem.State.Attack 
{
    /// <summary>
    /// NormalAttack1状態 
    /// </summary>
    public class NormalAttack1State : PlayerBaseState<AttackStateEnum> 
    {
        public NormalAttack1State(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            Debug.Log("NormalAttack1State: Enter");
            
            //TODO: Animatorで管理できるように作る
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == AttackStateEnum.NormalAttack1)
            {
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Step))
            {
                // コンボ終了 ステップ
            }
            
            if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Skill))
            {
                // コンボ終了 スキル
            }
            
            if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Jump) 
                && InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Attack))
            {
                StateMachine.ChangeState(AttackStateEnum.AirAttack1); // 空中①
                return;
            }

            if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Action))
            {
                StateMachine.ChangeState(AttackStateEnum.ThrowSword); // 刀投げ
                return;
            }
            
            if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Attack))
            {
                StateMachine.ChangeState(AttackStateEnum.NormalAttack2); // 通常②
                return;
            }

            if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Jump))
            {
                // コンボ終了 通常ジャンプ
            }
            
            if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Guard))
            {
                // コンボ終了 ガード
            }
            
            await UniTask.Yield();
        }
    }
}