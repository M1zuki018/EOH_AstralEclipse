using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// ダッシュ回避状態
    /// </summary>
    public class StepState : PlayerBaseState<BaseStateEnum>
    {
        public StepState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        // アクションを設定
        private bool _isJumping = false;
        
        // イベント登録
        private Action _onJumped;
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            Debug.Log("Step State: Enter");
            //TODO: アニメーション再生
            
            _onJumped = () => _isJumping = true;
            
            InputProcessor.OnJump += _onJumped;
            
            BlackBoard.ApplyGravity = true; // 多少浮く場合があるので重力を加えておく
            ActionHandler.Step(); // ステップ
            
            var delayTask = UniTask.Delay(TimeSpan.FromSeconds(0.5f)); // 0.5秒待機するタスク
            var jumpTask = UniTask.WaitUntil(() => _isJumping); // ジャンプ入力を待機するタスク
            
            await UniTask.WhenAny(delayTask, jumpTask); // どちらかが先に完了したら進む
            
            // 移動入力がなくなれば Idle へ。入力があれば Move 遷移する
            if (BlackBoard.MoveDirection.sqrMagnitude < 0.01f)
            {
                StateMachine.ChangeState(BaseStateEnum.Idle);
                return;
            }
            else
            {
                StateMachine.ChangeState(BaseStateEnum.Move);
                return;
            }
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            // ジャンプ入力があり、地面についていた場合 Jump へ
            if (_isJumping && BlackBoard.IsGrounded)
            {
                StateMachine.ChangeState(BaseStateEnum.Jump);
                return;
            }
            
            // フラグをリセット
            _isJumping = false;
            
            await UniTask.Yield();
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            // ステップ終了処理
            BlackBoard.ApplyGravity = false;
            BlackBoard.Velocity = new Vector3(0, -0.1f, 0); //確実に地面につくように少し下向きの力を加える
            
            // 状態をリセット
            _isJumping = false;

            // イベントを解除
            InputProcessor.OnStep -= _onJumped;
            
            await UniTask.Yield();
        }
    }
}
