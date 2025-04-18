using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 攻撃の向き補正を行うスクリプト
/// </summary>
public class AdjustDirection : ViewBase
{
    [SerializeField] private Transform _player;
    public Transform Target { get; private set; } // ターゲットのTransform
    private Quaternion _initialRotation;
    public Quaternion InitialRotation => _initialRotation;

    public override UniTask OnStart()
    {
        _initialRotation = transform.rotation;
        return base.OnStart();
    }

    /// <summary>
    /// 外のクラスからターゲットを設定する
    /// </summary>
    public void SetTarget(Transform target)
    {
        Target = target;
    }
    
    /// <summary>
    /// ターゲット方向にプレイヤーを回転させる
    /// </summary>
    public void AdjustDirectionToTarget()
    {
        if (Target == null) return;

        Vector3 direction = (Target.position - _player.transform.position).normalized; //敵の方向を取得
        direction.y = 0; // 垂直方向の補正。Y軸を無視する
        
        if (direction.sqrMagnitude > 0.01f) // 回転すべき角度が小さすぎない場合
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    
    /// <summary>
    /// ターゲット方向にプレイヤーを回転させる
    /// </summary>
    public void AdjustDirectionToTarget(float correctionAngle)
    {
        if (Target == null) return;

        Vector3 direction = (Target.position - _player.transform.position).normalized; //敵の方向を取得
        direction.y = 0; // 垂直方向の補正。Y軸を無視する
        
        if (direction.sqrMagnitude > 0.01f) // 回転すべき角度が小さすぎない場合
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation = new Quaternion(targetRotation.x, targetRotation.y + correctionAngle, targetRotation.z, targetRotation.w);
            _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    
    /// <summary>
    /// ルートモーション使用時用。即座にターゲット方向にプレイヤーを回転させる
    /// </summary>
    public void AdjustDirectionToTargetEarly()
    {
        if (Target == null) return;

        Vector3 direction = (Target.position - _player.transform.position).normalized; //敵の方向を取得
        direction.y = 0; // 垂直方向の補正。Y軸を無視する
        
        if (direction.sqrMagnitude > 0.01f) // 回転すべき角度が小さすぎない場合
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _player.transform.rotation = targetRotation;
        }
    }
}
