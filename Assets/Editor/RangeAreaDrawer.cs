using UnityEditor;
using UnityEngine;

/// <summary>
/// 最小値から最大値までのスライダーを提供
/// ※int型の変数のみ適用される
/// </summary>
[CustomPropertyDrawer(typeof(RangeAreaAttribute))]
internal sealed class RangeAreaDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RangeAreaAttribute rangeArea = (RangeAreaAttribute)attribute;

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.IntSlider(position, property, rangeArea.min, rangeArea.max, label);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
