using UnityEngine;

/// <summary>
/// キャラクターのステータス情報を書き込むスクリプタブルオブジェクト
/// </summary>
[CreateAssetMenu(fileName = "StatusSO", menuName = "Create StatusSO")]
public class StatusSO : ScriptableObject
{
    public int MaxHP;
    public int MaxTP;
    public int AttackPower;
    public int Defense;
}
