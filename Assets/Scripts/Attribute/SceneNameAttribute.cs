using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// シーン名をプルダウンで選択できるようにする属性
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class SceneNameAttribute : PropertyAttribute
{
    public bool includeAllScenes;

    public SceneNameAttribute(bool includeAllScenes = false)
    {
        this.includeAllScenes = includeAllScenes;
    }
}