using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// ガード状態
    /// </summary>
    public class GuardState : PlayerBaseState<BaseStateEnum>
    {
        public GuardState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        private bool _guardCancel;
        
        private Action _onGuardCancel;
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            Debug.Log("Guard State: Enter");
         
            _onGuardCancel = () => _guardCancel = true;
            
            InputProcessor.OnGuard += _onGuardCancel;

            BlackBoard.IsGuarding = true;
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Guard)
            {
                ActionHandler.Guard();
                
                if (_guardCancel) // ガードキャンセル
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
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// ステートから出るときの処理
        /// </summary>
        public override async UniTask Exit()
        {
            BlackBoard.IsGuarding = false;
            
            // 状態をリセット
            _guardCancel = false;
            
            // イベントを解除
            InputProcessor.OnGuard -= _onGuardCancel;
            
            await UniTask.Yield();
        }
    }
}
