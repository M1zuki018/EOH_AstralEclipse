using Cysharp.Threading.Tasks;
using PlayerSystem.Fight;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// レーザービームを管理するクラス
/// </summary>
public class LaserParticle : MonoBehaviour, IBossAttack
{
    public string AttackName => "HorizontalLaser";

    [SerializeField] private ParticleSystem _laserEffect;
    [SerializeField] private TriggerControl _triggerControl;
    [SerializeField] private EnemyCombat _combat;
    [SerializeField] private int _damageMag = 1;
    
    public GameObject LaserEffect => _laserEffect.gameObject;


    private void Start()
    {
        _triggerControl.OnTrigger += HandleHit; //登録
    }

    private void OnDestroy()
    {
        _triggerControl.OnTrigger -= HandleHit; //解除
    }

    /// <summary>
    /// レーザーを放つ前、魔法陣だけアクティブにする
    /// </summary>
    public void Sty(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// レーザーを放つ
    /// </summary>
    public void Fire(Vector3 firePoint)
    {
        transform.position = firePoint;
        gameObject.transform.GetChild(0).gameObject.SetActive(true); //ビームオブジェクトもアクティブにする
        _laserEffect.Play();
    }

    /// <summary>
    /// レーザーを止める
    /// </summary>
    public void Stop()
    {
        _laserEffect.Stop();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void HandleHit(Collider other)
    {
        if (other.tag == "Player")
        {
            var target = other.gameObject.GetComponent<IDamageable>();
            if (target != null)
            {
                _combat.DamageHandler.ApplyDamage(
                    target:target, //攻撃対象
                    baseDamage: _combat.BaseAttackPower * _damageMag, //攻撃力 
                    defense:0,  //相手の防御力
                    attacker:gameObject); //攻撃を加えるキャラクターのゲームオブジェクト
            }
        }
    }
    
    public UniTask Fire()
    {
        throw new System.NotImplementedException();
    }
}