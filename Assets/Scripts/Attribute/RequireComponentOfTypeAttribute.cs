using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class RequireComponentOfTypeAttribute : PropertyAttribute
{
    public Type RequiredType;

    public RequireComponentOfTypeAttribute(Type requiredType)
    {
        RequiredType = requiredType;
    }
}
