using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BarrierTriggerとBarrierオブジェクトを管理するシステム
/// </summary>
public class BariierSystem : MonoBehaviour
{
    [SerializeField] List<BariierTrigger> _bariierTriggers = new List<BariierTrigger>();
    [SerializeField] GameObject _bariierA;
    [SerializeField] GameObject _bariierB;
    
    private void Start()
    {
        foreach (var trigger in _bariierTriggers)
        {
            trigger.Initialize(this);
        }
        
        HideBariier();
    }

    /// <summary>
    /// バリアの位置を変更する
    /// </summary>
    public void SetBariierPos(Vector3 posA, Vector3 posB)
    {
        _bariierA.transform.position = posA;
        _bariierB.transform.position = posB;
        _bariierA.SetActive(true);
        _bariierB.SetActive(true);
    }

    /// <summary>
    /// バリアオブジェクトを非アクティブにする
    /// </summary>
    public void HideBariier()
    {
        _bariierA.SetActive(false);
        _bariierB.SetActive(false);
    }
}
