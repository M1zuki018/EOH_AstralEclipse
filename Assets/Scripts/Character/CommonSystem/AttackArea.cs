using PlayerSystem.Fight;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private IDamageable _damageable;
    
    /// <summary>
    /// 当たり判定の中に入った時の処理
    /// </summary>
    /// <param name="other">攻撃を受けた対象</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out _damageable))
        {
            _damageable.TakeDamage(10, gameObject); //ダメージと攻撃する側のオブジェクトを渡す
        }
    }
}
