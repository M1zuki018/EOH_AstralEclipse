using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspectorの変数を変更不可にする
/// </summary>
[CustomPropertyDrawer(typeof(DisableAttribute))]
public class DisableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property, label);
        EditorGUI.EndDisabledGroup();
    }
}