using System;
using UnityEngine;

/// <summary>
/// Barrierを移動させる時に使用するTrigger
/// </summary>
public class BarrierTrigger : MonoBehaviour
{
    [SerializeField] private BarrierConst barrierPosA;
    [SerializeField] private BarrierConst barrierPosB;
    private BarrierSystem _barrierSystem;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(BarrierSystem barrierSystem)
    {
        _barrierSystem = barrierSystem;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _barrierSystem.SetBarrierPos(barrierPosA, barrierPosB); //バリアの位置を変更する
            enabled = false; //以降このスクリプトは使用しない
        }
    }
}

[Serializable]
public class BarrierConst
{
    public Vector3 Position;
    public bool IsRotate;
}