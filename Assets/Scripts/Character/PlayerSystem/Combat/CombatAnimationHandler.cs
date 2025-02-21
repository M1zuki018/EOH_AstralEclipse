using UnityEngine;

/// <summary>
/// アニメーションイベントを補助するクラス
/// </summary>
public class CombatAnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField, Comment("当たり判定制御クラス")] private AttackHitDetector _hitDetector;
    [SerializeField, Comment("体の方向を敵に向けるクラス")] private AdjustDirection _adjustDirection;
    [SerializeField, Comment("エフェクト")] private EffectPool _effectPool;
    
    #region アニメーション

    /// <summary>ルートモーションを適用</summary>
    public void OnRootMotionEnable() => _animator.applyRootMotion = true;

    /// <summary>ルートモーションを解除</summary>
    public void OnRootMotionDisable() => _animator.applyRootMotion = false;
    
    /// <summary>アニメーション速度を変更</summary>
    public void OnSetAttackSpeed(float speed) => _animator.SetFloat("AttackSpeed", speed);

    /// <summary>体を敵の方向に向ける</summary>
    public void OnAdjustDirectionEnable(float correctionAngle) => _adjustDirection.AdjustDirectionToTarget(correctionAngle);
    
    /// <summary>すぐに体を敵の方向に向ける</summary>
    public void OnAdjustDirectionToTargetEarly() => _adjustDirection.AdjustDirectionToTargetEarly();
    
    #endregion

    #region SE

    /// <summary>SEを再生する</summary>
    public void PlaySE(int index) => AudioManager.Instance?.PlaySE(index);

    #endregion

    #region 当たり判定
    
    /// <summary>当たり判定を発生させる</summary>
    public void OnHitDetect(HitDetectionInfo info) => _hitDetector.DetectHit(info);

    #endregion
    
    #region カメラエフェクト

    /// <summary>ダッシュエフェクト</summary>
    public void DashEffect() => CameraManager.Instance?.DashEffect();
    
    /// <summary>回転斬りのエフェクト</summary>
    public void TurnEffect() => CameraManager.Instance?.TurnEffect();
    
    /// <summary>効果を元に戻す</summary>
    public void EndDashEffect() => CameraManager.Instance?.EndDashEffect();
    
    /// <summary>ヒットストップ</summary>
    public void ApplyHitStop(float hitStop) => CameraManager.Instance?.ApplyHitStop(hitStop);

    #endregion

    #region エフェクト

    /// <summary>エフェクトを表示する</summary>
    public void EffectEnable(Vector3 pos, Quaternion rot) => _effectPool.GetEffect(pos, rot);

    #endregion
}
