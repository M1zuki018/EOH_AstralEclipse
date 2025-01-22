using UnityEngine;

/// <summary>
/// プレイヤーの中心となるクラス
/// </summary>
public class PlayerBrain : CharacterBase
{
    protected override void HandleDamage(int damage, GameObject attacker)
    {
        _uiManager.UpdatePlayerHP(damage);
        //TODO:エネミーHPバーの管理方法を考える
    }

    protected override void HandleDeath(GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}に倒された！");
        //TODO:死亡エフェクト等の処理
    }
}
