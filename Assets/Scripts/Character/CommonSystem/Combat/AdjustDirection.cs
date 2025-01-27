using UnityEngine;

/// <summary>
/// 攻撃の向き補正を行うスクリプト
/// </summary>
public class AdjustDirection : MonoBehaviour
{
    [SerializeField] private Transform _player;
    public Transform Target { get; set; } // ターゲットのTransform
    
    /// <summary>
    /// ターゲット方向にプレイヤーを回転させる
    /// </summary>
    public void AdjustDirectionToTarget()
    {
        if (Target == null) return;

        Vector3 direction = (Target.position - _player.transform.position).normalized; //敵の方向を取得
        Debug.Log(direction);
        direction.y = 0; // 垂直方向の補正。Y軸を無視する
        
        if (direction.sqrMagnitude > 0.01f) // 回転すべき角度が小さすぎない場合
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
