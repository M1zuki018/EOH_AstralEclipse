using UnityEngine;

/// <summary>
/// 足跡エフェクトを生成する機能
/// </summary>
public class FootprintEffect : MonoBehaviour
{
    [SerializeField] private GameObject _effectPrefab;

    /// <summary>
    /// エフェクトを生成する
    /// </summary>
    public void CreateFootprint()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            // エフェクトを生成
            GameObject effect = Instantiate(_effectPrefab, hit.point, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }
}