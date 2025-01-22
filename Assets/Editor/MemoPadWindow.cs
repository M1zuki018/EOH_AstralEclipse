using UnityEditor;
using UnityEngine;

/// <summary>
/// 「Window」メニューからアクセスできるカスタムウィンドウ
/// メモを入力して保存できます
/// </summary>
public class MemoPadWindow : EditorWindow
{
    private string memoText = ""; // メモの内容
    private Vector2 scrollPosition;

    [MenuItem("Window/Custom/Memo Pad")]
    public static void ShowWindow()
    {
        // ウィンドウを開く
        var window = GetWindow<MemoPadWindow>("Memo Pad");
        window.minSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        // 保存されたメモをロード
        memoText = EditorPrefs.GetString("MemoPadText", "");
    }

    private void OnDisable()
    {
        // メモを保存
        EditorPrefs.SetString("MemoPadText", memoText);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Memo Pad", EditorStyles.boldLabel);

        // スクロール可能なテキストエリア
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        memoText = EditorGUILayout.TextArea(memoText, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        // 保存ボタン
        if (GUILayout.Button("Save"))
        {
            EditorPrefs.SetString("MemoPadText", memoText);
            Debug.Log("Memo saved!");
        }
    }
}
