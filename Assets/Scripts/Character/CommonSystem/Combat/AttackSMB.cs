using PlayerSystem.Fight;
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
    private Transform _actor;
    private AttackAdjustBase _attackCorrection; //移動・回転補正を行うクラス
    
    //アニメーションが開始されたとき
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_combat == null) //nullだったら取得する
        {
            _combat = animator.GetComponent<ICombat>();
            _actor = animator.transform;
        }
        
        animator.applyRootMotion = false; //補正をかけるため一度ルートモーションを無効にする
        
        if (animator.CompareTag("Player"))
        {
            _attackCorrection = GetAttackCorrectionByPlayer(animator); //補正クラスを取得する
            _attackCorrection.StartAttack();
        }
        else if (animator.CompareTag("Enemy"))
        {
            _attackCorrection = GetAttackCorrectionByEnemy(animator);
            _attackCorrection.StartAttack();
        }
        
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
            _attackCorrection.CorrectMovement(_actor.forward);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //中断処理
        _attackCorrection.CancelAttack();
    }

    /// <summary>
    /// 攻撃タイプごとの補正クラスを決定する
    /// </summary>
    private AttackAdjustBase GetAttackCorrectionByPlayer(Animator animator)
    {
        AttackAdjustBase collection = null;
        
        switch (_attackIndex)
        {
            case 0:
                collection = animator.GetComponent<NormalAttack_First>();  //1段目
                break;
            case 1:
                collection =  animator.GetComponent<NormalAttack_Second>(); //2段目
                break;
            case 2:
                collection = animator.GetComponent<NormalAttack_Kick>(); //3段目
                break;
            case 3:
                collection = animator.GetComponent<NormalAttack_Turn>(); //4段目
                break;
            case 4:
                collection = animator.GetComponent<NormalAttack_End>(); //5段目
                break;
            default:
                collection =  animator.GetComponent<NormalAttackCorrection>(); //デフォルトの攻撃補正
                break;
        }

        if (collection == null)
        {
            Debug.LogWarning("補正クラスが取得できませんでした");
        }

        return collection;
    }

    /// <summary>
    /// 攻撃タイプごとの補正クラスを決定する（Enemy用）
    /// </summary>
    private AttackAdjustBase GetAttackCorrectionByEnemy(Animator animator)
    {
        AttackAdjustBase collection = null;
        
        switch (_attackIndex)
        {
            case 0:
                collection = animator.GetComponent<EnemyNormalAttack_Swiping>();  //殴り
                break;
            case 1:
                collection =  animator.GetComponent<EnemyNormalAttack_JumpAttack>(); //飛び上がる
                break;
            case 2:
                collection =  animator.GetComponent<BossAttack_First>(); //ボスの攻撃
                break;
            case 3:
                collection =  animator.GetComponent<BossAttack_Second>();
                break;
            case 4:
                collection =  animator.GetComponent<BossAttack_Tried>();
                break;
            case 5:
                collection =  animator.GetComponent<BossAttack_Fourth>();
                break;
            default:
                collection =  animator.GetComponent<NormalAttackCorrection>(); //デフォルトの攻撃補正
                break;
        }

        if (collection == null)
        {
            Debug.LogWarning("補正クラスが取得できませんでした");
        }

        return collection;
    }
}
