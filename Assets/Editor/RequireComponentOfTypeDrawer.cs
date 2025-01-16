using UnityEditor;
using UnityEngine;

/// <summary>
/// 特定のコンポーネントを指定して参照する
/// 宣言は以下のようにできる
/// [RequireComponentOfType(typeof(Rigidbody))]
/// public GameObject _rbObj;
/// </summary>
[CustomPropertyDrawer(typeof(RequireComponentOfTypeAttribute))]
public class RequireComponentOfTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attribute = (RequireComponentOfTypeAttribute)base.attribute;

        // フィールドがObject型かどうかを確認
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            // 現在の値をObjectとして取得
            Object currentObject = property.objectReferenceValue;

            // 指定された型のみを受け付けるObjectFieldを表示
            EditorGUI.BeginChangeCheck();
            Object selectedObject = EditorGUI.ObjectField(
                position,
                label,
                currentObject,
                attribute.RequiredType, // 型を制限
                true // シーン内のオブジェクトのみ許可
            );

            // 変更があれば値を更新
            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = selectedObject;
            }
        }
        else
        {
            //対象外の場合エラーメッセージを表示
            EditorGUI.LabelField(position, label.text, "Error: この属性を使用できるのはオブジェクトフィールドのみです");
        }
    }
}
