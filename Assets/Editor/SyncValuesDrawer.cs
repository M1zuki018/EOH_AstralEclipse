using UnityEditor;
using UnityEngine;

/// <summary>
/// 複数のオブジェクト間で特定の値を一括編集する
/// </summary>
[CustomPropertyDrawer(typeof(SyncValuesAttribute))]
public class SyncValuesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //現在の値を表示
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();
        
        //プロパティの値を表示
        object value = null;

        switch (property.propertyType)
        {
            case SerializedPropertyType.Float:
                property.floatValue = EditorGUI.FloatField(position, label, property.floatValue);
                break;

            case SerializedPropertyType.Integer:
                property.intValue = EditorGUI.IntField(position, label, property.intValue);
                break;

            case SerializedPropertyType.Boolean:
                property.boolValue = EditorGUI.Toggle(position, label, property.boolValue);
                break;

            case SerializedPropertyType.String:
                property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
                break;

            default:
                EditorGUI.LabelField(position, label.text, "サポートされていない型です");
                break;
        }
        
        // 値が変更された場合にすべての選択されたオブジェクトの値を更新
        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }
}
