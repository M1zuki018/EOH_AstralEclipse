using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// プレイヤーのUIを管理する補助クラス
/// </summary>
public class PlayerUIController
{
    private PlayerBlackBoard _bb;

    public PlayerUIController(PlayerBlackBoard bb)
    {
        _bb = bb;
    }
    
    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialized()
    {
        UIManager.Instance?.InitializePlayerHP(_bb.Status.MaxHP, _bb.CurrentHP);
        UIManager.Instance?.InitializePlayerWill(_bb.Status.Will, _bb.CurrentWill);
        UIManager.Instance?.InitializePlayerTP(_bb.Status.MaxTP, _bb.CurrentTP);
    }

    /// <summary>
    /// プレイヤーの位置にダメージ表記を表示
    /// </summary>
    public void ShowDamage(int damage, Transform transform) => UIManager.Instance?.ShowDamageAmount(damage, transform);
    
    /// <summary>
    /// Willゲージを更新（ガード中）
    /// </summary>
    public void UpdateWill() => UIManager.Instance?.UpdatePlayerWill(_bb.CurrentWill);
    
    /// <summary>
    /// HPゲージを更新
    /// </summary>
    public void UpdateHP() => UIManager.Instance?.UpdatePlayerHP(_bb.CurrentHP);

    /// <summary>
    /// 死んだ時のUIの処理
    /// </summary>
    public void WhenDeath()
    {
        UIManager.Instance.HidePlayerBattleUI();
        UIManager.Instance.HideRightUI();
        UIManager.Instance.HideLockOnUI();
        UIManager.Instance.HideBossUI();
    }
    
    /// <summary>
    /// 死亡時のパネルを非表示にする
    /// </summary>
    public void ShowDeathPanel() => UIManager.Instance.ShowDeathPanel();
}
