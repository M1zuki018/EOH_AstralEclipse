using Cysharp.Threading.Tasks;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// 垂直レーザーのオブジェクトにアタッチするクラス
/// </summary>
public class VirticalLaserControl : MonoBehaviour, IBossAttack
{
    public string AttackName => "VirticalLaser";
    
    [SerializeField] private float _survivalTime = 10f;
    private ICombat _combat;
    private void OnEnable()
    {
        Destroy(gameObject, _survivalTime); //生存時間
    }

    public void SetCombat(ICombat newCombat)
    {
        _combat = newCombat;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
    
    public UniTask Fire()
    {
        throw new System.NotImplementedException();
    }
}
