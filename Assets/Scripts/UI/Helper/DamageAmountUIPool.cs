using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// ダメージ量を表示するテキストのオブジェクトプールを作成する
/// </summary>
public class DamageAmountUIPool : MonoBehaviour
{
    [SerializeField] private DamageAmountUI _damageAmountPrefab; // ダメージUIのプレハブ
    [SerializeField] private int _initialPoolSize = 10; // 初期プールサイズ

    private Queue<DamageAmountUI> pool = new Queue<DamageAmountUI>();

    private void Awake()
    {
        // 初期プールの作成
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateNewInstance();
        }
    }

    /// <summary>
    /// 新しくインスタンスを作成する
    /// </summary>
    private DamageAmountUI CreateNewInstance()
    {
        DamageAmountUI instance = Instantiate(_damageAmountPrefab, transform);
        instance.gameObject.SetActive(false);
        instance.Initialize(this);
        pool.Enqueue(instance);
        return instance;
    }

    /// <summary>
    /// インスタンスを取り出す
    /// </summary>
    public DamageAmountUI GetInstance()
    {
        if (pool.Count > 0)
        {
            DamageAmountUI instance = pool.Dequeue();
            instance.gameObject.SetActive(true);
            return instance;
        }
        return CreateNewInstance(); // プールが空なら新しく作成
    }

    /// <summary>
    /// 表示したオブジェクトをプールに戻す
    /// </summary>
    public void ReturnToPool(DamageAmountUI instance)
    {
        instance.Hide();
        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }
}
