using PlayerSystem.ActionFunction;
using UnityEngine;

/// <summary>
/// ウォールラン機能
/// </summary>
public class WallRunFunction : MonoBehaviour, IWallRunable
{
    [SerializeField] private float _runForceMultiplier = 1.7f; //壁走り中の加速倍率
    [SerializeField] private float _characterRotationZ = 20f; //キャラクターを傾ける量
    [SerializeField] private float _rotationDuration = 0.15f; //回転が完了するまでの時間
    [SerializeField] private float _wallRunDuration = 3.0f; //壁走り可能な時間

    /// <summary>
    /// ウォールラン機能
    /// </summary>
    public void WallRun(Vector3 direction, bool isLeft)
    {
        RotateCharacterForWallRun(direction, isLeft);
    }

    /// <summary>
    /// 自然に見えるようにキャラクターの体を傾ける
    /// </summary>
    private void RotateCharacterForWallRun(Vector3 direction, bool isLeft)
    {
        float angleZ = isLeft ? -_characterRotationZ : _characterRotationZ;
    }

    public void WallRun()
    {
        throw new System.NotImplementedException();
    }
}
