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
    [SerializeField] private int _damageMag = 4;
    private Transform _target;
    private Collider _collider;
    private ICombat _combat;
    private Vector3 _initialPosition;
    private Renderer _renderer;

    private void OnEnable()
    {
        _collider = GetComponent<Collider>();
        _renderer = transform.GetChild(0).GetComponent<Renderer>(); //子オブジェクト（腕）のRendererを取得する
        _collider.enabled = false;
    }

    public void SetCombat(ICombat combat, Transform target)
    {
        _combat = combat;
        _target = target;
        _renderer.materials[0].SetFloat("_Cutoff", 0.1f); //初期値にセット
        
        Fire().Forget();
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
                _collider.enabled = true;
                CameraManager.Instance.TriggerCameraShake();
            });
        
        
        await UniTask.Delay(1000);
        
        await UpdateDissolveValue(1, 1f);
        
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var target = other.gameObject.GetComponent<IDamageable>();
        if (target != null)
        {
            _combat.DamageHandler.ApplyDamage(
                target: target, //攻撃対象
                baseDamage: _combat.BaseAttackPower * _damageMag, //攻撃力 
                defense: 0, //相手の防御力
                attacker: gameObject); //攻撃を加えるキャラクターのゲームオブジェクト
        }
    }
    
    /// <summary>
    /// マテリアルのディゾルブ効果の値を徐々に変更する
    /// </summary>
    private async UniTask UpdateDissolveValue(float cutOff, float duration)
    {
        Debug.Log(_renderer.materials[0]);
        await _renderer.materials[0].DOFloat(cutOff, "_Cutoff", duration).SetEase(Ease.InOutQuad).ToUniTask();
    }
}
