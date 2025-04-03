/// <summary>
/// プレイヤーのカメラ操作を管理するクラス
/// </summary>
public class PlayerCameraController
{
    public void Shake() => CameraManager.Instance?.TriggerCameraShake();
    public void WhenDeath() => CameraManager.Instance.PlayerDeath();
}
