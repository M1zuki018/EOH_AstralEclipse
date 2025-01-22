using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// 攻撃判定のエリアにつけるクラス
/// </summary>
public class AttackArea : MonoBehaviour
{
    private ICombat _me;
    private void Start()
    {
        _me = GetComponentInParent<ICombat>();
    }

    /// <summary>
    /// 当たり判定の中に入った時の処理
    /// </summary>
    /// <param name="other">攻撃を受けた対象</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {
           // _me.Attack(health as IDamageable);
        }
    }
}
