using UnityEngine;

/// <summary>
/// ミニマップ描画用のカメラを管理する
/// </summary>
public class MinimapFollow : MonoBehaviour
{
    [SerializeField] Transform _player; // プレイヤーのTransformを設定
    [SerializeField] Vector3 _offset;   // カメラのオフセット値

    void LateUpdate()
    {
        if (_player != null)
        {
            Vector3 newPosition = _player.position + _offset;
            newPosition.y = _offset.y; // 高さを一定に保つ
            transform.position = newPosition;
        }
    }
}