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
        private int _isSkill = -1;
        private bool _isStep = false;

        private Action _onAttack;
        private Action<int> _onSkill;
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
            _onSkill = index => _isSkill = index;
            _onStep = () => _isStep = true;
            
            // イベント登録
            InputProcessor.OnAttack += _onAttack;
            InputProcessor.OnSkill += _onSkill;
            InputProcessor.OnStep += _onStep;

            ActionHandler.Jump();
            BlackBoard.ApplyGravity = true;
            
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
                if (BlackBoard.IsGrounded && BlackBoard.Velocity.y < 0)
                {
                    // 移動入力がなければ Idle へ
                    if (BlackBoard.MoveDirection.sqrMagnitude < 0.01f)
                    {
                        StateMachine.ChangeState(BaseStateEnum.Idle);
                        return;
                    }
                    // 移動入力があれば Move へ
                    else
                    {
                        StateMachine.ChangeState(BaseStateEnum.Move);
                        return;
                    }
                }
                
                // スキル番号がデフォルトから変わっていたら Skill へ
                if (_isSkill != -1)
                {
                    BlackBoard.UsingSkillIndex = _isSkill;
                    StateMachine.ChangeState(BaseStateEnum.Skill);
                    return;
                }
                
                // 攻撃入力があれば Attack へ
                if (_isAttacking)
                {
                    StateMachine.ChangeState(BaseStateEnum.Attack);
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
                
                // フラグをリセット
                _isStep = false;
                
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            // ジャンプ終了処理
            BlackBoard.IsJumping = false;
            BlackBoard.Velocity = new Vector3(0, -0.1f, 0); //確実に地面につくように少し下向きの力を加える
            BlackBoard.ApplyGravity = false;
            
            // 状態をリセット
            _isAttacking = false;
            _isSkill = -1;
            _isStep = false;

            // イベントを解除
            InputProcessor.OnAttack -= _onAttack;
            InputProcessor.OnSkill -= _onSkill;
            InputProcessor.OnStep -= _onStep;
            
            await UniTask.Yield();
        }
    }
}
