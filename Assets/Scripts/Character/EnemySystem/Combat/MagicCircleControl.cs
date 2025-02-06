using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パターン3の攻撃で使用する魔法陣の管理スクリプト
/// </summary>
public class MagicCircleControl : MonoBehaviour
{
    [SerializeField] private GameObject _energyPrefab;
    private List<GameObject> _energies = new List<GameObject>();

    private void OnEnable()
    {
        //中央の1つ
        Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        _energies.Add(Instantiate(_energyPrefab, position, Quaternion.identity, transform)); 
        
        //外側に円周上に等間隔で配置する
        float radius = 3f; //外側のエネルギー弾の配置半径
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // ランダムな角度を作成

        for (int i = 0; i < 5; i++)
        {
            randomAngle += 72 * Mathf.Deg2Rad; //72度ずつ回転させて等間隔に並ぶようにする
            position = new Vector3(
                transform.position.x + Mathf.Cos(randomAngle) * radius,
                transform.position.y + Mathf.Sin(randomAngle) * radius,
                transform.position.z - 1 
            );

            _energies.Add(Instantiate(_energyPrefab, position, Quaternion.identity, transform)); 
        }
    }
}
