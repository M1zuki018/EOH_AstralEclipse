using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class ReadOnlyOnRuntimeAttribute : PropertyAttribute { }
