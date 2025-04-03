using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// エフェクトのオブジェクトプールを管理する
/// </summary>
public class EffectPool : ViewBase
{
    [SerializeField] private GameObject _effectPrefab; // 剣のエフェクトプレハブ
    [SerializeField] private int _poolSize = 5; // プールの初期サイズ

    private Queue<GameObject> _effectPool = new Queue<GameObject>();

    public override UniTask OnStart()
    {
        // 事前にエフェクトオブジェクトを生成し、プールに格納
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject effect = Instantiate(_effectPrefab, transform.parent);
            effect.SetActive(false);
            _effectPool.Enqueue(effect);
        }
        
        return base.OnStart();
    }
    
    /// <summary>
    /// エフェクトを取得（必要な位置・角度に配置）
    /// </summary>
    public void GetEffect(Vector3 position, Quaternion rotation)
    {
        GameObject effect;

        if (_effectPool.Count > 0)
        {
            effect = _effectPool.Dequeue();
        }
        else
        {
            // プールが足りなくなったら新しく生成
            effect = Instantiate(_effectPrefab, transform.parent);
        }

        effect.transform.localPosition = position;
        effect.transform.localRotation = rotation;
        effect.SetActive(true);

        // 一定時間後にエフェクトをプールに戻す
        StartCoroutine(ReturnToPool(effect, 0.9f));
    }

    // エフェクトを非アクティブにしてプールへ戻す
    private IEnumerator ReturnToPool(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        effect.SetActive(false);
        _effectPool.Enqueue(effect);
    }
}