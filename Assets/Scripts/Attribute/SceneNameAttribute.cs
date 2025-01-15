using System;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class SceneNameAttribute : PropertyAttribute
{
    public bool includeAllScenes;

    public SceneNameAttribute(bool includeAllScenes = false)
    {
        this.includeAllScenes = includeAllScenes;
    }
}