using UnityEngine;

/// <summary>
/// 垂直レーザーのオブジェクトにアタッチするクラス
/// </summary>
public class VirticalLaserControl : MonoBehaviour
{
    [SerializeField] private float _survivalTime = 10f;
    private void OnEnable()
    {
        Destroy(this.gameObject, _survivalTime); //生存時間
    }
}
