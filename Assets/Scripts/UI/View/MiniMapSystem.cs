using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// ミニマップを管理するクラス
/// </summary>
public class MiniMapSystem : MonoBehaviour
{
    [SerializeField, HighlightIfNull] private Image _light;
    [SerializeField, HighlightIfNull] private Camera _mainCamera;

    private void Update()
    {
        RotateLight();
    }

    /// <summary>
    /// ミニマップのライトを回転させる
    /// </summary>
    private void RotateLight()
    {
        _light.transform.rotation = Quaternion.Euler(0,0,-_mainCamera.transform.rotation.eulerAngles.y);
    }
}
