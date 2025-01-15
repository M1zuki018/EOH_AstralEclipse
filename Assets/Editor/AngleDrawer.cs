using UnityEditor;
using UnityEngine;

/// <summary>
/// 角度をノブで調整できるようにする
/// 宣言は[Angle(ノブの大きさ)]
/// </summary>
[CustomPropertyDrawer(typeof(AngleAttribute))]
public class AngleDrawer : PropertyDrawer
{
    private AngleAttribute angleAttribute => (AngleAttribute)attribute;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // フロート型以外は警告を表示
        if (property.propertyType != SerializedPropertyType.Float)
        {
            EditorGUI.LabelField(position, label.text, "角度にはfloat型を使ってください");
            return;
        }
        
        // ノブの描画
        var knobRect = new Rect(position.x + position.width - angleAttribute.knobSize, position.y,
            angleAttribute.knobSize, angleAttribute.knobSize);

        var valueRect = new Rect(position.x, position.y, position.width - angleAttribute.knobSize - 10,
            position.height);

        EditorGUI.LabelField(valueRect, label); // ラベル描画

        property.floatValue = angleAttribute.Knob(
            knobRect,
            property.floatValue,
            0f, // 最小値
            360f, // 最大値
            "°", // 単位
            Color.gray, // 背景色
            Color.green, // アクティブ時の色
            true // 値を表示
        );
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = base.GetPropertyHeight(property, label);
        return property.propertyType != SerializedPropertyType.Float ? height : angleAttribute.knobSize + 4;
    }
}
