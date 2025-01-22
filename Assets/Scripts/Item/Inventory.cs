using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのインベントリを管理するクラス
/// </summary>
public class Inventory : MonoBehaviour
{
    private HashSet<string> _collectedKeys = new HashSet<string>(); //集めたキーのハッシュセット

    /// <summary>
    /// ハッシュセットに入手したキーを追加する
    /// </summary>
    public void AddKey(string keyName)
    {
        _collectedKeys.Add(keyName);
        Debug.Log("キー取得: " + keyName);
    }

    /// <summary>
    /// 3つキーが揃っていたらtrueを返す
    /// </summary>
    public bool HasAllKeys()
    {
        return _collectedKeys.Count >= 3;
    }
}