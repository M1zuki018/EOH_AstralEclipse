using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class AngleAttribute : PropertyAttribute
{
    private readonly MethodInfo knobMethodInfo = typeof(EditorGUI).GetMethod("Knob",
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

    public float knobSize = 50; //defaultのサイズ

    public AngleAttribute(float knobSize = 50)
    {
        this.knobSize = knobSize;
    }
    
    public float Knob(Rect position, float currentValue, float start, float end, string unit, 
        Color backgroundColor, Color activeColor, bool showValue)
    {
        var controlID = GUIUtility.GetControlID("Knob".GetHashCode(), FocusType.Passive, position);
        
        var invoke = knobMethodInfo.Invoke(null, new object[]
        {
            position, new Vector2(knobSize, knobSize), currentValue,
            start, end, unit, backgroundColor,
            activeColor, showValue, controlID
        });

        return (float)(invoke ?? currentValue);
    }
}