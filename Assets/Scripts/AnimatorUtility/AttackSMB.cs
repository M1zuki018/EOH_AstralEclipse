using UnityEngine;

/// <summary>
/// 攻撃モーション用のSMB
/// </summary>
public class AttackSMB : StateMachineBehaviour
{
    [Header("初期設定")] 
    [SerializeField] private int _attackIndex;

    private PlayerCombat _combat;
    
    //アニメーションが開始されたとき
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_combat == null) //nullだったら取得する
        {
            _combat = animator.GetComponent<PlayerCombat>();
        }
        
        _combat.PerformAttack(_attackIndex); //攻撃処理メソッドを呼ぶ
    }
}
