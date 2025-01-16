using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ShowIfAttribute : PropertyAttribute
{
    public string ConditionPropertyName;

    public ShowIfAttribute(string conditionPropertyName)
    {
        ConditionPropertyName = conditionPropertyName;
    }
}
