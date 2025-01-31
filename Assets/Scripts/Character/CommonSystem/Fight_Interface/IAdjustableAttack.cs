using UnityEngine;

/// <summary>
/// 攻撃アニメーションの補正処理を調整するためのクラスが継承するべきインターフェース
/// </summary>
public interface IAdjustableAttack
{
    /// <summary>
    /// 攻撃開始時に呼び出される処理
    /// </summary>
    void StartAttack();
}
