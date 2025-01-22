using UnityEngine;

/// <summary>
/// プレイヤーの中心となるクラス（体力・死亡管理）
/// </summary>
[RequireComponent(typeof(PlayerMovement), typeof(Health), typeof(PlayerCombat))]
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerBrain : CharacterBase
{
    private void Start()
    {
        _uiManager.InitializePlayerHP(GetMaxHP(), GetCurrentHP());
    }

    protected override void HandleDamage(int damage, GameObject attacker)
    {
        TakeDamage(damage, attacker); //IHealthのダメージを食らう処理が呼ばれる
        _uiManager.UpdatePlayerHP(GetCurrentHP());
        //TODO:エネミーHPバーの管理方法を考える
    }

    protected override void HandleDeath(GameObject attacker)
    {
        Debug.Log($"{gameObject.name}は{attacker.name}に倒された！");
        //TODO:死亡エフェクト等の処理
    }
}
