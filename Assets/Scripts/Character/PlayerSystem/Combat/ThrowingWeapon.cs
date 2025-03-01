using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// 武器投げ・回収を行うクラス
/// </summary>
public class ThrowingWeapon
{
    private PlayerBlackBoard _bb;
    private readonly GameObject _weaponObj; // 武器のオブジェクト
    
    private readonly float _throwForce = 20f; // 投げる力
    private Rigidbody _rb;
    private Collider _col;
    private Transform _weaponParent; // 武器オブジェクトの親

    public ThrowingWeapon(PlayerBlackBoard bb, GameObject weaponObj)
    {
        _bb = bb;
        _weaponObj = weaponObj;
        
        _rb = weaponObj.GetComponent<Rigidbody>();
        _col = weaponObj.GetComponent<Collider>();
        _weaponParent = _weaponObj.transform.parent; // 親のオブジェクトの情報を保存しておく
    }

    /// <summary>
    /// 武器を投げる
    /// </summary>
    public void ThrowWeapon()
    {
        if (_rb == null)
        {
            _rb = _weaponObj.GetComponent<Rigidbody>();
            _col = _weaponObj.GetComponent<Collider>();
        }

        _weaponObj.transform.SetParent(null); // 親子関係解消
        
        _rb.useGravity = true;
        _col.enabled = true;
        _rb.transform.rotation = Quaternion.Euler(0, 0, 0);
        _rb.AddForce(Vector3.forward * _throwForce, ForceMode.Impulse);
    }

    /// <summary>
    /// 武器を回収する
    /// </summary>
    public void RecastWeapon()
    {
        if (_rb == null)
        {
            _rb = _weaponObj.GetComponent<Rigidbody>();
        }
    }
}
