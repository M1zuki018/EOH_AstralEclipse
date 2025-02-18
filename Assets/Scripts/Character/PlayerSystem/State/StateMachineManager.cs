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
        private BaseStateEnum _currentState;

        #region bool型プロパティ

        public bool IsIdle => _currentState == BaseStateEnum.Idle;
        public bool IsWalking => _currentState == BaseStateEnum.Walk;
        public bool IsRunning => _currentState == BaseStateEnum.Run;
        public bool IsJumping => _currentState == BaseStateEnum.Jump;
        public bool IsStep => _currentState == BaseStateEnum.Step;
        public bool IsGuard => _currentState == BaseStateEnum.Guard;
        public bool IsParry => _currentState == BaseStateEnum.Parry;
        public bool IsAttacking => _currentState == BaseStateEnum.Attack;
        public bool IsCounter => _currentState == BaseStateEnum.Counter;
        public bool IsHit => _currentState == BaseStateEnum.Hit;
        public bool IsDead => _currentState == BaseStateEnum.Dead;
        public bool IsPerformance => _currentState == BaseStateEnum.Performance;

        #endregion
       
    
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
                //_isAttacking = true;
                //_isIdle = false;
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.W))
            {
                //_isRunning = true;
                //_isIdle = false;
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