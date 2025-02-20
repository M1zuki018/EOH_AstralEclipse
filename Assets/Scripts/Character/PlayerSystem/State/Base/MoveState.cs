using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniVRM10;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 移動状態
    /// </summary>
    public class MoveState : PlayerBaseState<BaseStateEnum>, IFixedUpdateState
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
            Debug.Log("MoveState Enter");
            
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
            Debug.Log("MoveState Execute");
            await UniTask.Yield();
            
            while (StateMachine.CurrentState.Value == BaseStateEnum.Move)
            {
                // 移動入力がなくなれば Idle へ
                if (BlackBoard.MoveDirection.sqrMagnitude < 0.01f)
                {
                    StateMachine.ChangeState(BaseStateEnum.Idle);
                    return;
                }

                // ジャンプ入力があり、地面についていた場合 Jump へ
                if (_isJumping && BlackBoard.IsGrounded)
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
                
                _isJumping = false; // ジャンプフラグをリセット

                await UniTask.Yield();
            }
        }
        
        public override async UniTask FixedExecute()
        {
            await UniTask.Yield();
            
            /* //TODO:FixedUpdateで更新しているはずなのに、ここで呼ぶFixedUpdateと外のものの振る舞いが違う
            while (StateMachine.CurrentState.Value == BaseStateEnum.Move)
            {
                //ActionHandler.Move();
                
                // 移動入力がなくなれば Idle へ
                if (BlackBoard.MoveDirection.sqrMagnitude < 0.01f)
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
            */
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
