using UnityEngine;

/// <summary>
/// Barrierを移動させる時に使用するTrigger
/// </summary>
public class BariierTrigger : MonoBehaviour
{
    [SerializeField] private Vector3 _bariierPosA;
    [SerializeField] private Vector3 _bariierPosB;
    private BariierSystem _bariierSystem;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(BariierSystem bariierSystem)
    {
        _bariierSystem = bariierSystem;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _bariierSystem.SetBariierPos(_bariierPosA, _bariierPosB); //バリアの位置を変更する
            enabled = false; //以降このスクリプトは使用しない
        }
    }
}
