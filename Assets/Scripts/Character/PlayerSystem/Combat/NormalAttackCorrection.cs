using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// モーションのうちキャラクターの向きや移動を補正するクラス
/// </summary>
public class NormalAttackCorrection : AttackAdjustBase
{
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private bool _adjustDirection = true;

    public override void StartAttack()
    {
        _combat?.AdjustDirection.AdjustDirectionToTarget();  //向きの補正
        _hitDetector.DetectHit(_hitDetectionInfo.Collider, _hitDetectionInfo.Duration);
    }

    /// <summary>
    /// 移動補正の処理
    /// </summary>
    public override void CorrectMovement(Vector3 forwardDirection)
    {
        if (_adjustDirection)
        {
            _combat?.AdjustDirection.AdjustDirectionToTarget(); 
        }
        Vector3 move = forwardDirection * _moveSpeed * Time.deltaTime; // 移動量の計算
        _cc.Move(move);  // 実際の移動処理
    }
    
}
