using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ミニマップを管理するクラス
/// </summary>
public class MiniMapSystem : ViewBase
{
    [SerializeField, HighlightIfNull] private Image _light;
    [SerializeField, HighlightIfNull] private Camera _mainCamera;

    private void Update()
    {
        // ミニマップのライトを回転させる
        _light.transform.rotation = Quaternion.Euler(0,0,-_mainCamera.transform.rotation.eulerAngles.y);
    }
}
