using System;

/// <summary>
/// 攻撃補正の情報をまとめた構造体
/// </summary>
[Serializable]
public struct AttackData
{
    public float MoveSpeed;
    public bool AdjustDirection;
}
