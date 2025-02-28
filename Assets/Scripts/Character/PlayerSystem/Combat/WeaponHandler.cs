using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// 武器を構える/しまう処理を行うクラス
/// </summary>
public class WeaponHandler
{
    private PlayerBlackBoard _bb;
    private GameObject _weaponObj;
    
    public WeaponHandler(PlayerBlackBoard bb, GameObject weaponObj)
    {
        _bb = bb;
        _weaponObj = weaponObj;
        
        _weaponObj.SetActive(false);
        _bb.IsReadyArms = false;
    }
    
    /// <summary>
    /// 武器を構える
    /// </summary>
    public void HandleWeaponActivation()
    {
        _bb.AnimController.Combat.TriggerReadyForBattle(); // 武器を構えるアニメーション
        _bb.IsReadyArms = true;
        _weaponObj.SetActive(true); //武器のオブジェクトを表示する
        AudioManager.Instance.PlaySE(2);
        UIManager.Instance?.ShowPlayerBattleUI();
    }
    
    /// <summary>
    ///  武器をしまう処理
    /// </summary>
    public void HandleWeaponDeactivation()
    {
        _weaponObj.SetActive(false);
        _bb.IsReadyArms = false;
        AudioManager.Instance.PlaySE(2);
        UIManager.Instance?.HidePlayerBattleUI();
    }
}
