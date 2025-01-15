using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class RangeAreaAttribute : PropertyAttribute
{
    public readonly int min;
    public readonly int max;

    public RangeAreaAttribute(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}
