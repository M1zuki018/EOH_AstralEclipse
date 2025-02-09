using UnityEngine;
using System;

/// <summary>
/// RequireComponentのInterfaceバージョン
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public class RequireInterfaceAttribute : PropertyAttribute
{
    public Type RequiredInterface { get; }

    public RequireInterfaceAttribute(Type requiredInterface)
    {
        // インターフェースであることを確認
        if (!requiredInterface.IsInterface)
        {
            throw new ArgumentException("インターフェースである必要があります");
        }

        RequiredInterface = requiredInterface;
    }
}