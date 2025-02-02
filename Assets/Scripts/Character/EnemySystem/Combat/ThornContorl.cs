using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// 茨のプレハブにアタッチするクラス
/// </summary>
public class ThornContorl : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshRenderer;
    [SerializeField] private Mesh _cheangedMesh;
    private BoxCollider _collider;
    private ICombat _combat;

    private void OnEnable()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.enabled = false; //最初は判定をとらない
    }

    public void SetCombat(ICombat newCombat)
    {
        _combat = newCombat;
    }
    
    /// <summary>
    /// メッシュを変更する
    /// </summary>
    public void ChangedMesh()
    {
        _collider.enabled = true;
        _meshRenderer.mesh = _cheangedMesh;
        Destroy(this.gameObject, 2f); //時間経過でオブジェクトを削除する
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
