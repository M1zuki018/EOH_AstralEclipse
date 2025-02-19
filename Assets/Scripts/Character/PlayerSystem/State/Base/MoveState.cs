using System;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 移動状態
    /// </summary>
    public class MoveState : PlayerBaseState<BaseStateEnum>
    {
        public MoveState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        private bool _isJumping = false;
        private bool _isAttacking = false;
        private bool _isGuard = false;
        
        private Action _onJump;
        private Action _onAttack;
        private Action _onGuard;
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            //TODO: アニメーション再生
            
            // アクションを設定
            _onJump = () => _isJumping = true;
            _onAttack = () => _isAttacking = true;
            _onGuard = () => _isGuard = true;
            
            // イベント登録
            InputProcessor.OnJump += _onJump;
            InputProcessor.OnAttack += _onAttack;
            InputProcessor.OnGuard += _onGuard;
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {

            while (StateMachine.CurrentState.Value == BaseStateEnum.Move)
            {
                // 移動入力がなくなれば Idle へ
                if (BlackBoard.MoveDirection.magnitude == 0)
                {
                    StateMachine.ChangeState(BaseStateEnum.Idle);
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

                // 防御入力があれば Guard へ
                if (_isGuard)
                {
                    StateMachine.ChangeState(BaseStateEnum.Guard);
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
            _isJumping = false;
            _isAttacking = false;
            _isGuard = false;

            // イベントを解除
            InputProcessor.OnJump -= _onJump;
            InputProcessor.OnAttack -= _onAttack;
            InputProcessor.OnGuard -= _onGuard;
            
            await UniTask.Yield();
        }
    }
}
