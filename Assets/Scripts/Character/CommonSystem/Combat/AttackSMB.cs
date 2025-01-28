using UnityEngine;

/// <summary>
/// 攻撃モーション用のSMB
/// </summary>
public class AttackSMB : StateMachineBehaviour
{
    [Header("初期設定")] 
    [SerializeField, Comment("コンボの段階")] private int _attackIndex;
    [SerializeField, Comment("モーションの移動量")] private float _moveSpeed = 1.0f;
    [SerializeField, Comment("向きの補正をするか")] private bool _adjustDirection = true;
    [SerializeField, Comment("ルートモーションの使用")] private bool _useRootMotion = false; 
    
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

        animator.applyRootMotion = false; //一度ルートモーションは無効にする

        _combat.PerformAttack(_attackIndex); //攻撃処理メソッドを呼ぶ
        
        if (_useRootMotion) //ルートモーションを使用する場合
        {
            _combat?.AdjustDirection.AdjustDirectionToTarget();
            animator.applyRootMotion = true; //ルートモーションを有効
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_adjustDirection && !_useRootMotion) //補正が有効なら移動補正を行う
        {
            _combat?.AdjustDirection.AdjustDirectionToTarget();
            Vector3 forward = _player.forward; // 現在の向き
            Vector3 move = forward * _moveSpeed * Time.deltaTime; // 移動量計算
            _cc.Move(move);
        }
    }
}
