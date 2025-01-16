using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 入力される文字列が特定のフォーマットに従っているかをチェックする
/// ファイル名や特定の識別子を扱う場合などに
/// [SerializeField, RegexValidation(@"^\w+$", "英数字のみを使用できます")]
/// private string _fileName;
/// </summary>
[CustomPropertyDrawer(typeof(RegexValidationAttribute))]
public class RegexValidationDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RegexValidationAttribute regexAttribute = (RegexValidationAttribute)attribute;

        if (property.propertyType == SerializedPropertyType.String)
        {
            string value = property.stringValue;
            EditorGUI.PropertyField(position, property, label);

            if (!Regex.IsMatch(value, regexAttribute.Pattern))
            {
                EditorGUI.LabelField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height),
                    regexAttribute.ErrorMessage, EditorStyles.helpBox);
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Error: この属性はstring型のみ使用できます");
        }
    }
}
