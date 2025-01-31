using UnityEngine;

/// <summary>
/// 攻撃補正の基底クラス
/// </summary>
public abstract class AttackAdjustBase : MonoBehaviour, IAdjustableAttack, IAttackCorrection
{
    protected Animator _animator; //プレイヤーのAnimator
    protected CharacterController _cc; //プレイヤーのCharacterController
    protected Transform _target; //ロックオン中の敵
    
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _cc = GetComponent<CharacterController>();
    }

    public abstract void StartAttack(Transform target); //攻撃時に呼び出される処理
    public abstract void CorrectMovement(Vector3 forwardDirection); //移動補正
    public abstract void AdjustDirectionToTarget(); //回転補正
}