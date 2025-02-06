using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// 攻撃補正の基底クラス
/// </summary>
public abstract class AttackAdjustBase : MonoBehaviour, IAdjustableAttack, IAttackCorrection
{
    protected Animator _animator; //プレイヤーのAnimator
    protected CharacterController _cc; //プレイヤーのCharacterController
    protected Transform _target; //ロックオン中の敵
    protected AttackHitDetector _hitDetector; //当たり判定の処理を行うクラス
    protected ICombat _combat;
    protected AdjustDirection _adjustDirection;
    protected EffectPool _effectPool;
    
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _cc = GetComponent<CharacterController>();
        _hitDetector = GetComponentInChildren<AttackHitDetector>();
        _combat = GetComponent<ICombat>();
        _adjustDirection = GetComponentInChildren<AdjustDirection>();
        _effectPool = GameObject.FindGameObjectWithTag("Effect").GetComponent<EffectPool>();
    }

    public abstract void StartAttack(); //攻撃時に呼び出される処理
    public abstract void CorrectMovement(Vector3 forwardDirection); //移動補正
}