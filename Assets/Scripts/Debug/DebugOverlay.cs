using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UE風のログを表示するクラス
/// </summary>
public class DebugOverlay : MonoBehaviour
{
    private static List<string> logs = new List<string>();
    private static GUIStyle style;
    
    private void Awake()
    {
        style = new GUIStyle();
        style.fontSize = 50;
        style.normal.textColor = Color.white;
    }

    public static void Log(string message)
    {
        logs.Add($"<color=#{ColorUtility.ToHtmlStringRGB(new Color(0f, 0.6f, 0.9f))}>{message}</color>");
        if (logs.Count > 10) // 表示するログの最大数
        {
            logs.RemoveAt(0);
        }
    }

    private void OnGUI()
    {
        float y = 10;
        foreach (string log in logs)
        {
            GUI.Label(new Rect(10, y, 500, 55), log, style);
            y += 55;
        }
    }
}