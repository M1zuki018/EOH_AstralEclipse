
using UnityEngine;

/// <summary>
/// 攻撃補正を行うためのインターフェース
/// 攻撃補正に関係する情報を持っている
/// </summary>
public interface IAttackCorrection
{
    /// <summary>移動補正</summary>
    void CorrectMovement(Vector3 forwardDirection);

    /// <summary>回転補正</summary>
    void AdjustDirectionToTarget();
}
