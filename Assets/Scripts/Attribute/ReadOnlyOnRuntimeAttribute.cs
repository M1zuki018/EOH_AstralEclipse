using System;
using UnityEngine;

/// <summary>
/// 実行中はInspectorの値を変更できないようにする属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class ReadOnlyOnRuntimeAttribute : PropertyAttribute { }
