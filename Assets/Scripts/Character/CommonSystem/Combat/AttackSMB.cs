using PlayerSystem.Fight;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 攻撃モーション用のStateMachineBehavior
/// 攻撃の開始・終了とアニメーション管理を行う
/// </summary>
public class AttackSMB : StateMachineBehaviour
{
    [Header("初期設定")] 
    [SerializeField, Comment("コンボの段階")] private int _attackIndex;
    [SerializeField, Comment("ルートモーションの使用")] private bool _useRootMotion = false; 
    
    private ICombat _combat;
    private CharacterController _cc;
    private Transform _player;
    private AttackAdjustBase _attackCorrection; //移動・回転補正を行うクラス
    
    //アニメーションが開始されたとき
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_combat == null) //nullだったら取得する
        {
            _combat = animator.GetComponent<ICombat>();
            _cc = animator.GetComponent<CharacterController>();
            _player = animator.transform;
        }
        
        animator.applyRootMotion = false; //補正をかけるため一度ルートモーションを無効にする
        _attackCorrection = GetAttackCorrection(animator); //補正クラスを取得する
        _attackCorrection.StartAttack();
        
        if (_useRootMotion) //ルートモーションを使用する場合
        {
            animator.applyRootMotion = true;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_attackCorrection != null)
        {
            //移動補正処理を呼び出す
            _attackCorrection.CorrectMovement(_player.forward);
        }
    }

    /// <summary>
    /// 攻撃タイプごとの補正クラスを決定する
    /// </summary>
    private AttackAdjustBase GetAttackCorrection(Animator animator)
    {
        switch (_attackIndex)
        {
            case 0:
                return animator.GetComponent<NormalAttack_First>();  //1段目
            case 1:
                return animator.GetComponent<NormalAttack_Second>(); //2段目
            case 2:
                return animator.GetComponent<NormalAttack_Kick>(); //3段目
            case 3:
                return animator.GetComponent<NormalAttack_Turn>(); //4段目
            case 4:
                return animator.GetComponent<NormalAttack_End>(); //5段目
            default:
                return animator.GetComponent<NormalAttackCorrection>(); //デフォルトの攻撃補正
        }
    }
}
