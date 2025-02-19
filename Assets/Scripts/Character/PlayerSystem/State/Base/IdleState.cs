using System;
using Cysharp.Threading.Tasks;
using PlayerSystem.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Unit = UniRx.Unit;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 待機状態
    /// </summary>
    public class IdleState : BaseState<BaseStateEnum>
    {
        public IdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        
        private PlayerInputProcessor _inputProcessor;
        private PlayerBlackBoard _blackboard;
        private PlayerActionHandler _actionHandler;
        
        private bool _isJumping = false;
        private bool _isAttacking = false;
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            //TODO: 待機アニメーション再生

            _inputProcessor.OnJump += () => _isJumping = true;
            _inputProcessor.OnAttack += () => _isAttacking = true;
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Idle)
            {
                // 移動入力があれば Walk へ
                if (_blackboard.MoveDirection.magnitude > 0)
                {
                    StateMachine.ChangeState(BaseStateEnum.Walk);
                    return;
                }

                // ジャンプ入力があれば Jump へ
                if (_isJumping)
                {
                    StateMachine.ChangeState(BaseStateEnum.Jump);
                    return;
                }

                // 攻撃入力があれば Attack へ
                if (_isAttacking)
                {
                    StateMachine.ChangeState(BaseStateEnum.Attack);
                    return;
                }

                // フレームを待つ
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            _inputProcessor.OnJump -= () => _isJumping = false;
            _inputProcessor.OnAttack -= () => _isAttacking = false;
            
            await UniTask.CompletedTask;
        }
    }

}