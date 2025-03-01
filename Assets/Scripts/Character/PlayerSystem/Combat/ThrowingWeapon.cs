using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// 武器投げ・回収を行うクラス
/// </summary>
public class ThrowingWeapon
{
    private PlayerBlackBoard _bb;
    private readonly GameObject _weaponObj; // 武器のオブジェクト
    
    private Rigidbody _rb;
    private Collider _col;
    private Transform _weaponParent; // 武器オブジェクトの親
    private Vector3 _initialLocalPos; // 初期位置

    public ThrowingWeapon(PlayerBlackBoard bb, GameObject weaponObj)
    {
        _bb = bb;
        _weaponObj = weaponObj;
        
        _rb = weaponObj.GetComponent<Rigidbody>();
        _col = weaponObj.GetComponent<Collider>();
        _weaponParent = _weaponObj.transform.parent; // 親のオブジェクトの情報を保存しておく
        _initialLocalPos = _weaponObj.transform.localPosition;
    }

    /// <summary>
    /// 武器を投げる
    /// </summary>
    public void ThrowWeapon()
    {
        // 参照が取れていなかった場合
        if (_rb == null) _rb = _weaponObj.GetComponent<Rigidbody>();
        if (_col == null) _col = _weaponObj.GetComponent<Collider>();

        _weaponObj.transform.SetParent(null); // 親子関係解消
        
        _rb.isKinematic = false; // 演算する
        _col.enabled = true; // 刀オブジェクトに当たり判定を適用
        _rb.transform.rotation = Quaternion.Euler(0, 0, 0); // 水平に飛び出すように回転を修正する
        _rb.AddForce(Vector3.forward * _bb.Data.ThrowForce, ForceMode.Impulse);
        
        _bb.IsThrown = true;
    }

    /// <summary>
    /// 武器を回収する
    /// </summary>
    public void RecastWeapon()
    {
        // 参照が取れていなかった場合
        if (_rb == null) _rb = _weaponObj.GetComponent<Rigidbody>();
        if (_col == null) _col = _weaponObj.GetComponent<Collider>();
        
        _rb.isKinematic = true; // 物理演算を止める
        _col.enabled = false; // 当たり判定は切っておく
        
        // 位置の調整
        _weaponObj.transform.SetParent(_weaponParent);
        _weaponObj.transform.localPosition = _initialLocalPos;
        _weaponObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        
        _bb.IsThrown = false;
    }
}
