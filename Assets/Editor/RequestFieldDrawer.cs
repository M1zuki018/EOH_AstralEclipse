using UnityEditor;
using UnityEngine;

/// <summary>
/// オブジェクトが未割当の場合変数名にマーカーする
/// </summary>
[CustomPropertyDrawer(typeof(HighlightIfNullAttribute))]
public class RequestFieldEditor : PropertyDrawer
{
    private static readonly Color ErrorFieldColor = new Color(0.6f, 0, 0);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (IsShowError(property))
            EditorGUI.DrawRect(position, ErrorFieldColor);

        base.OnGUI(position, property, label);
        EditorGUI.PropertyField(position, property);
    }

    private static bool IsShowError(SerializedProperty property)
    {
        return property.objectReferenceValue == null;
    }
}
