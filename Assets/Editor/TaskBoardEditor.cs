using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// 簡易的なタスクボードを提供します
/// </summary>
public class TaskBoardEditor : EditorWindow
{
    private List<Task> tasks = new List<Task>();
    private string savePath;

    [MenuItem("Window/Task Board")]
    public static void ShowWindow()
    {
        GetWindow<TaskBoardEditor>("Task Board");
    }

    private void OnEnable()
    {
        savePath = Path.Combine(Application.persistentDataPath, "TaskBoard.json");
        LoadTasks();
    }

    private void OnGUI()
    {
        GUILayout.Label("Task Board", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Task"))
        {
            tasks.Add(new Task());
            SaveTasks();
        }

        for (int i = 0; i < tasks.Count; i++)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            
            tasks[i].title = EditorGUILayout.TextField("Title", tasks[i].title);
            
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                tasks.RemoveAt(i);
                SaveTasks();
                break;
            }
            GUILayout.EndHorizontal();

            for (int j = 0; j < tasks[i].requirements.Count; j++)
            {
                GUILayout.BeginHorizontal();
                tasks[i].requirements[j].isChecked = EditorGUILayout.Toggle(tasks[i].requirements[j].isChecked, GUILayout.Width(20));
                tasks[i].requirements[j].text = EditorGUILayout.TextField(tasks[i].requirements[j].text);
                
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    tasks[i].requirements.RemoveAt(j);
                    SaveTasks();
                    break;
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Requirement"))
            {
                tasks[i].requirements.Add(new Requirement());
                SaveTasks();
            }

            GUILayout.EndVertical();
        }

        if (GUILayout.Button("Save"))
        {
            SaveTasks();
        }
    }

    private void SaveTasks()
    {
        string json = JsonUtility.ToJson(new TaskList { tasks = this.tasks }, true);
        File.WriteAllText(savePath, json);
    }

    private void LoadTasks()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            tasks = JsonUtility.FromJson<TaskList>(json)?.tasks ?? new List<Task>();
        }
    }

    [Serializable]
    public class TaskList
    {
        public List<Task> tasks;
    }

    [Serializable]
    public class Task
    {
        public string title = "New Task";
        public List<Requirement> requirements = new List<Requirement>();
    }

    [Serializable]
    public class Requirement
    {
        public bool isChecked = false;
        public string text = "New Requirement";
    }
}
