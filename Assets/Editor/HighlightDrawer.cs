using UnityEditor;
using UnityEngine;

/// <summary>
/// 特定のフィールドやラベルを色付けする
/// 宣言↓
/// [SerializeField, Highlight(1, 0.5f, 0.5f)] private float sharedSpeed;
/// </summary>
[CustomPropertyDrawer(typeof(HighlightAttribute))]
public class HighlightDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HighlightAttribute highlight = (HighlightAttribute)attribute;

        // 元のGUIカラーを保存
        Color originalBackgroundColor = GUI.backgroundColor;
        Color originalTextColor = GUI.contentColor;

        // カスタムカラーを適用
        GUI.backgroundColor = highlight.BackgroundColor;
        GUI.contentColor = highlight.TextColor;

        // フィールドを描画
        EditorGUI.PropertyField(position, property, label);

        // 元のカラーに戻す
        GUI.backgroundColor = originalBackgroundColor;
        GUI.contentColor = originalTextColor;
    }
}
