using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BarrierTriggerとBarrierオブジェクトを管理するシステム
/// </summary>
public class BarrierSystem : MonoBehaviour
{
    [SerializeField] List<BarrierTrigger> _barrierTriggers = new List<BarrierTrigger>();
    [SerializeField] GameObject _barrierA;
    [SerializeField] GameObject _barrierB;
    
    private void Start()
    {
        _barrierTriggers.ForEach(trigger => trigger.Initialize(this));
        HideBarrier();
    }

    /// <summary>
    /// バリアの位置を変更する
    /// </summary>
    public void SetBarrierPos(BarrierConst posA, BarrierConst posB)
    {
        SetBarrier(_barrierA, posA);
        SetBarrier(_barrierB, posB);
    }

    /// <summary>
    /// バリアの位置と回転を調整してアクティブにする
    /// </summary>
    private void SetBarrier(GameObject barrier, BarrierConst barrierConst)
    {
        if(barrier == null) return;
        
        barrier.transform.position = barrierConst.Position;
        barrier.transform.localRotation = barrierConst.IsRotate ? Quaternion.Euler(0,90,0) : Quaternion.identity;
        barrier.SetActive(true);
    }

    /// <summary>
    /// バリアオブジェクトを非アクティブにする
    /// </summary>
    public void HideBarrier()
    {
        _barrierA?.SetActive(false);
        _barrierB?.SetActive(false);
    }
}
