
using UnityEngine;

/// <summary>
/// 攻撃補正を行うためのインターフェース
/// </summary>
public interface IAttackCorrection
{
    /// <summary>移動補正</summary>
    void CorrectMovement(Vector3 forwardDirection);

    /// <summary>回転補正</summary>
    void AdjustDirectionToTarget();
}
