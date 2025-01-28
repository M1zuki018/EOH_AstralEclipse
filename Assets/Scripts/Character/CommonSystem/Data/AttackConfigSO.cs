using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackConfig" , menuName = "Create AttackConfig")]
public class AttackConfigSO : ScriptableObject
{
    public List<AttackData> AttackConfigs;

    /// <summary>
    /// AttackDataを返す
    /// </summary>
    public AttackData GetAttackConfig(int attackIndex)
    {
        return AttackConfigs[attackIndex];
    }
}
