using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace PlayerSystem.State.Base
{
    /// <summary>
    /// ガード状態
    /// </summary>
    public class GuardState : PlayerBaseState<BaseStateEnum>
    {
        public GuardState(IPlayerStateMachine stateMachine) : base(stateMachine) { }

        private ReactiveProperty<int> _countProp = new ReactiveProperty<int>(0); // パリィ判定用のカウンター
        
        /// <summary>
        /// ステートに入るときの処理
        /// </summary>
        public override async UniTask Enter()
        {
            ActionHandler.GuardStart(); // ガード処理を開始
            
            BlackBoard.ApplyGravity = true;
            BlackBoard.ParryReception = true; // パリィ受付開始
            
            _countProp.Value = 0; // countPropを初期化
            
            // countProp の変更を監視
            _countProp
                .DistinctUntilChanged() // 値が変更されたときのみ処理を実行
                .Where(count => count >= BlackBoard.Status.ParryReceptionTime) // パリィ受付終了時間に達したらやめる
                .Take(1) // 1回だけ処理を実行
                .Subscribe(count =>
                {
                    BlackBoard.ParryReception = false; // パリィ受付終了
                    Debug.Log("パリィ受付終了");
                });
            
            await UniTask.Yield();
        }

        /// <summary>
        /// 毎フレーム呼ばれる処理（状態遷移など）
        /// </summary>
        public override async UniTask Execute()
        {
            while (StateMachine.CurrentState.Value == BaseStateEnum.Guard)
            {
                _countProp.Value++;
               
                ActionHandler.Guard();

                // パリィ受付時間中に攻撃を食らったら、Parry状態に以降
                if (BlackBoard.ParryReception && BlackBoard.SuccessParry)
                {
                    BlackBoard.ParryReception = false;
                    BlackBoard.SuccessParry = false;
                    StateMachine.ChangeState(BaseStateEnum.Parry);
                    return;
                }
                
                // ガードブレイク状態
                if (BlackBoard.IsGuardBreak)
                {
                    StateMachine.ChangeState(BaseStateEnum.GuardBreak);
                    return;
                }

                // ガードをやめる
                if (InputProcessor.InputBuffer.GetBufferedInput(InputNameEnum.Guard)) 
                {
                    // 移動入力がなければ Idle へ
                    if (BlackBoard.MoveDirection.sqrMagnitude < 0.01f)
                    {
                        StateMachine.ChangeState(BaseStateEnum.Idle);
                        return;
                    }
                    
                    // 移動入力があれば Move へ
                    StateMachine.ChangeState(BaseStateEnum.Move);
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
            ActionHandler.GuardEnd(); // ガードをやめる処理を呼ぶ
            
            BlackBoard.ApplyGravity = false;
            BlackBoard.ParryReception = false;
            
            await UniTask.Yield();
        }
    }
}
