using UnityEngine;

/// <summary>
/// 攻撃モーション用のSMB
/// </summary>
public class AttackSMB : StateMachineBehaviour
{
    [Header("初期設定")] 
    [SerializeField, Comment("コンボの段階")] private int _attackIndex;
    [SerializeField, Comment("モーションの再生速度")] private float _moveSpeed = 1.0f;
    [SerializeField, Comment("向きの補正をするか")] private bool _adjustDirection = true;
    
    private PlayerCombat _combat;
    private CharacterController _cc;
    private Transform _player;
    
    //アニメーションが開始されたとき
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_combat == null) //nullだったら取得する
        {
            _combat = animator.GetComponent<PlayerCombat>();
            _cc = animator.GetComponent<CharacterController>();
            _player = animator.transform;
        }
        
        _combat.PerformAttack(_attackIndex); //攻撃処理メソッドを呼ぶ
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _combat.AdjustDirection.AdjustDirectionToTarget();
        if (_adjustDirection) //補正が有効で、補正が完了していたら移動補正を行う
        {
            Vector3 forward = _player.forward; // 現在の向き
            Vector3 move = forward * _moveSpeed * Time.deltaTime; // 移動量計算
            _cc.Move(move);
        }
    }
}
