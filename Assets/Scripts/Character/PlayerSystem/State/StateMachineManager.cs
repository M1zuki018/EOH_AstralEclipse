using Cysharp.Threading.Tasks;
using PlayerSystem.State.Base;
using UnityEngine;

namespace PlayerSystem.State
{
    /// <summary>
    /// プレイヤーの状態を管理し、ステートマシンの更新を行う
    /// </summary>
    public class StateMachineManager : MonoBehaviour
    {
        private StateMachine _stateMachine;
        public bool IsIdle => _isIdle;
        public bool IsRunning => _isRunning;
        public bool IsAttacking => _isAttacking;
    
        private bool _isIdle;
        private bool _isRunning;
        private bool _isAttacking;
    
        private void Start()
        {
            _stateMachine = new StateMachine(this);
            _stateMachine.TransitionToState(new IdleState(this)).Forget();
        }
    
        private void Update()
        {
            _stateMachine.Update().Forget();
    
            // 状態遷移をチェック（例として）
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                _isAttacking = true;
                _isIdle = false;
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.W))
            {
                _isRunning = true;
                _isIdle = false;
            }
        }
    
        public void TransitionToState(IState newState)
        {
            _stateMachine.TransitionToState(newState).Forget();
        }
    
        // アニメーションの設定例
        public void SetIdleAnimation()
        {
            // Idleアニメーションを設定
        }
    
        public void SetAttackAnimation()
        {
            // Attackアニメーションを設定
        }
    }
   
}