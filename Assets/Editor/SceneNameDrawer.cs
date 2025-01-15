using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// シーン名をポップアップから選択できるようにする
/// [SceneName(true)]と宣言した場合は、オフのシーンも含めて表示する
/// [SceneName] public string 変数名;
/// </summary>
[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SceneNameAttribute sceneNameAttribute = (SceneNameAttribute)attribute;
        
        var scenes = EditorBuildSettings.scenes; //ビルド設定からSceneリストを取得する

        var sceneNames = new List<string>();
        for (int i = 0; i < scenes.Length; i++)
        {
            if (sceneNameAttribute.includeAllScenes || scenes[i].enabled)
            {
                sceneNames.Add(System.IO.Path.GetFileNameWithoutExtension(scenes[i].path));
            }
        }
        
        int selectedIndex = Mathf.Max(0, sceneNames.IndexOf(property.stringValue)); //プロパティの現在の値を選択し、インデックスを選択
        selectedIndex = EditorGUI.Popup(position, selectedIndex, sceneNames.ToArray()); //ポップアップとして表示

        if (selectedIndex >= 0 && selectedIndex < sceneNames.Count)
        {
            property.stringValue = sceneNames[selectedIndex]; //プロパティの値を更新
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
