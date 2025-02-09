using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// ボスの頭上からの攻撃を管理するクラス
/// </summary>
public class AboveControl : MonoBehaviour, IBossAttack
{
    public string AttackName => "Above";
    
    [SerializeField] private GameObject _arm;
    private Transform _target;
    private Collider _collider;
    private ICombat _combat;
    private Vector3 _initialPosition;

    private void OnEnable()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
    }

    public void SetCombat(ICombat combat, Transform target)
    {
        _combat = combat;
        _target = target;
        
        Fire();
    }

    /// <summary>
    /// 攻撃時に呼び出すメソッド
    /// </summary>
    public async UniTask Fire()
    {
        transform.position = new Vector3(_target.position.x, 10, _target.position.z); //ターゲットの頭上に出現
        _initialPosition = _arm.transform.position;
        
        _arm.transform.DOMoveY(_initialPosition.y + 10, 0.8f)
            .SetEase(Ease.OutQuad);
        
        await UniTask.Delay(800); // 持ち上げ時間
        
        _arm.transform.DOMoveY(-3.5f, 0.3f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                //Instantiate(smashEffectPrefab, target.position, Quaternion.identity);
                _collider.enabled = true;
                CameraManager.Instance.TriggerCameraShake();
            });
        
        
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
}
