using UnityEngine;

/// <summary>
/// プレイヤーの設定の定数をまとめたスクリプタブルオブジェクト
/// </summary>
[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Create Scriptable Objects/PlayerSettingsSO")]
public class PlayerSettingsSO : ScriptableObject
{
    [Header("接地判定に関する設定")]
    [SerializeField, Comment("Rayの長さ")] private float _rayLength = 0.35f;
    [SerializeField, Comment("Rayをどれくらい身体にめり込ませるか")] private float _rayOffset = 0.1f;
    [SerializeField, Comment("Rayの判定に用いるLayer")] private LayerMask _layerMask = default;
    
    public float RayLength => _rayLength;
    public float RayOffset => _rayOffset;
    public LayerMask LayerMask => _layerMask;
}
