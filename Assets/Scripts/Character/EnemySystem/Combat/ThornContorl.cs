using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// 茨のプレハブにアタッチするクラス
/// </summary>
public class ThornContorl : MonoBehaviour
{
    [SerializeField] private GameObject _thorn; //棘のオブジェクト
    private Collider _collider;
    private ICombat _combat;

    private void OnEnable()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false; //最初は判定をとらない
        _thorn.SetActive(false); //棘のオブジェクトは最初は非表示にする
        //_thorn.transform.position = new Vector3(
        //    _thorn.transform.position.x, _thorn.transform.position.y, transform.position.z - 2f); //地中に埋める
    }

    public void SetCombat(ICombat newCombat)
    {
        _combat = newCombat;
    }
    
    /// <summary>
    /// 棘オブジェクトを表示する
    /// </summary>
    public async void ChangedMesh()
    {
        _collider.enabled = true;
        _thorn.SetActive(true);
        //_thorn.transform.DOMoveZ(0, 0.1f);
        
        await UniTask.Delay(2000);
        
        //一定時間経過したらオブジェクトを削除する
        Destroy(this.gameObject);
        //_thorn.transform.DOMoveZ(-2, 0.5f).OnComplete(() => Destroy(this.gameObject));
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
