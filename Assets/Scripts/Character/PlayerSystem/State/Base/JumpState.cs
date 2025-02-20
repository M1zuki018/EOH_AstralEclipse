using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// ジャンプ状態
    /// </summary>
    public class JumpState : PlayerBaseState<BaseStateEnum>
    {
        public JumpState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        private bool _isAttacking = false;
        private bool _isStep = false;

        private Action _onAttack;
        private Action _onStep;
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            Debug.Log("JumpState: Enter");
            
            //TODO: アニメーション再生
            
            // アクションを設定
            _onAttack = () => _isAttacking = true;
            _onStep = () => _isStep = true;
            
            // イベント登録
            InputProcessor.OnAttack += _onAttack;
            InputProcessor.OnStep += _onStep;

            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Jump)
            {
                //TODO: ジャンプが終わった時の入力量に応じてIdle/Moveに遷移する
                
                // 攻撃入力があれば Attack へ
                if (_isAttacking)
                {
                    StateMachine.ChangeState(BaseStateEnum.Attack);
                    return;
                }

                // ダッシュ回避入力があれば Step へ
                if (_isStep)
                {
                    StateMachine.ChangeState(BaseStateEnum.Step);
                    return;
                }
                
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            // 状態をリセット
            _isAttacking = false;
            _isStep = false;

            // イベントを解除
            InputProcessor.OnAttack -= _onAttack;
            InputProcessor.OnStep -= _onStep;
            
            await UniTask.Yield();
        }
    }
}
