using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class HighlightAttribute : PropertyAttribute
{
    public Color BackgroundColor { get; }
    public Color TextColor { get; }

    public HighlightAttribute(float r, float g, float b, float textR = 1, float textG = 1, float textB = 1)
    {
        BackgroundColor = new Color(r, g, b);
        TextColor = new Color(textR, textG, textB);
    }
}
