using System;
using System.Reflection;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _gameManagerPrefab;

    private void Start()
    {
        AutoInstantiate();
    }

    private void AutoInstantiate()
    {
        // クラス内のすべてのフィールドを取得
        FieldInfo[] fields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // [SerializeField] が付いていて、型が GameObject なら処理する
            if (Attribute.IsDefined(field, typeof(SerializeField)) && field.FieldType == typeof(GameObject))
            {
                GameObject prefab = (GameObject)field.GetValue(this); // プレハブ取得

                if (prefab != null)
                {
                    // プレハブ名を取得して_を削除
                    string varName = field.Name.TrimStart('_');

                    // 推測されるコンポーネントの型を取得
                    Type inferredType = FindViewBaseType(prefab);
                    if (inferredType != null)
                    {
                        // `GameObjectUtility.Instantiate<T>(プレハブ名)` を自動実行
                        MethodInfo instantiateMethod = typeof(GameObjectUtility).GetMethod("Instantiate")
                            .MakeGenericMethod(inferredType);
                        var instance = instantiateMethod.Invoke(null, new object[] { prefab });

                        Debug.Log($"{varName} ({inferredType.Name}) を自動生成しました！");
                    }
                    else
                    {
                        Debug.LogWarning($"{varName} のコンポーネントの型が特定できません。スキップします。");
                    }
                }
            }
        }
    }

    // プレハブの最初の `ViewBase` 派生コンポーネントの型を取得
    private Type FindViewBaseType(GameObject prefab)
    {
        Component[] components = prefab.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component is ViewBase)
            {
                return component.GetType();
            }
        }
        return null;
    }
}
