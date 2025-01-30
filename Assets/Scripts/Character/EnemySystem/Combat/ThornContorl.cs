using UnityEngine;

/// <summary>
/// 茨のプレハブにアタッチするクラス
/// </summary>
public class ThornContorl : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshRenderer;
    [SerializeField] private Mesh _cheangedMesh;
    
    /// <summary>
    /// メッシュを変更する
    /// </summary>
    public void ChangedMesh()
    {
        _meshRenderer.mesh = _cheangedMesh;
        Destroy(this.gameObject, 2f); //時間経過でオブジェクトを削除する
    }
}
