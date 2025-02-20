using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// 待機状態
    /// </summary>
    public class IdleState : PlayerBaseState<BaseStateEnum>
    {
        public IdleState(IPlayerStateMachine stateMachine) : base(stateMachine) { }
        
        // アクションを設定
        private bool _isJumping = false;
        private bool _isStep = false;
        private bool _isAttacking = false;
        private bool _isGuard = false;
        
        // イベント登録
        private Action _onJump;
        private Action _onStep;
        private Action _onAttack;
        private Action _onGuard;
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            //TODO: アニメーション再生

            Debug.Log("Idle entered");
            
            _onJump = () => _isJumping = true;
            _onStep = () => _isStep = true;
            _onAttack = () => _isAttacking = true;
            _onGuard = () => _isGuard = true;
            
            InputProcessor.OnJump += _onJump;
            InputProcessor.OnStep += _onStep;
            InputProcessor.OnAttack += _onAttack;
            InputProcessor.OnGuard += _onGuard;
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Idle)
            {
                //ActionHandler.Move();
                
                // 移動入力があれば Move へ
                if (BlackBoard.MoveDirection.sqrMagnitude > 0.01f)
                {
                    StateMachine.ChangeState(BaseStateEnum.Move);
                    return;
                }

                // ジャンプ入力があり、地面についていた場合 Jump へ
                if (_isJumping && BlackBoard.IsGrounded)
                {
                    StateMachine.ChangeState(BaseStateEnum.Jump);
                    return;
                }

                //　ステップ入力があり、ステップ回数がゼロ以上あったら Step へ
                if (_isStep)
                {
                    if (BlackBoard.CurrentSteps > 0)
                    {
                        StateMachine.ChangeState(BaseStateEnum.Step);
                    }
                    else
                    {
                        Debug.Log("ステップカウントが足りません！");
                    }
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
                
                // フラグをリセット
                _isStep = false;
                _isJumping = false;

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
            _isStep = false;
            _isAttacking = false;
            _isGuard = false;

            // イベントを解除
            InputProcessor.OnJump -= _onJump;
            InputProcessor.OnStep -= _onJump;
            InputProcessor.OnAttack -= _onAttack;
            InputProcessor.OnGuard -= _onGuard;
            
            await UniTask.CompletedTask;
        }
    }

}