using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)] // フィールドに適用されるように修正
public class DisableAttribute : PropertyAttribute { }