using Cysharp.Threading.Tasks;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// ボスの頭上からの攻撃を管理するクラス
/// </summary>
public class AboveControl : MonoBehaviour, IBossAttack
{
    public string AttackName => "Above";
    
    [SerializeField] private GameObject _arm;
    private Collider _collider;
    private ICombat _combat;

    private void OnEnable()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
        _arm.SetActive(false); //最初は非表示
    }

    public void SetCombat(ICombat combat)
    {
        _combat = combat;
    }

    /// <summary>
    /// 攻撃時に呼び出すメソッド
    /// </summary>
    public async void Attack()
    {
        _collider.enabled = true;
        _arm.SetActive(true);
        
        await UniTask.Delay(2500);
        
        Destroy(gameObject);
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

    
    public UniTask Fire()
    {
        throw new System.NotImplementedException();
    }
}
