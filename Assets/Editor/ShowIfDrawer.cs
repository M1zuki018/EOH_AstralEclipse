using UnityEditor;
using UnityEngine;

/// <summary>
/// 条件が満たされた場合のみフィールドをInspectorに表示する
/// [SerializeField] private bool showDetails;
/// [SerializeField, ShowIf("showDetails")] private string _tag;
/// </summary>
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var showIfAttribute = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIfAttribute.ConditionPropertyName);

        if (conditionProperty != null && conditionProperty.boolValue)
        {
            EditorGUI.PropertyField(position, conditionProperty, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var showIfAttribute = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIfAttribute.ConditionPropertyName);

        if (conditionProperty != null && !conditionProperty.boolValue)
        {
            return 0;
        }
        
        return base.GetPropertyHeight(property, label);
    }
}
