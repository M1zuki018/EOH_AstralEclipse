using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// ボスの頭上からの攻撃を管理するクラス
/// </summary>
public class AboveControl : MonoBehaviour
{
    private Collider _collider;
    private ICombat _combat;

    private void OnEnable()
    {
        _collider = GetComponent<Collider>();
    }

    public void SetCombat(ICombat combat)
    {
        _combat = combat;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var target = other.gameObject.GetComponent<IDamageable>();
        if (target != null)
        {
            _combat.DamageHandler.ApplyDamage(
                target: target, //攻撃対象
                baseDamage: _combat.BaseAttackPower, //攻撃力 
                defense: 0, //相手の防御力
                attacker: gameObject); //攻撃を加えるキャラクターのゲームオブジェクト
        }
    }
}
