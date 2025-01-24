using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのインベントリを管理するクラス
/// </summary>
public class Inventory : MonoBehaviour
{
    private HashSet<string> _collectedKeys = new HashSet<string>(); //集めたキーのハッシュセット
    private PlayerCombat _combat;
    private void Start()
    {
        _combat = GetComponentInParent<PlayerCombat>(); //UIManagerにアクセスするため
        _combat._uiManager.UpdateQuestText(
            $"Collect the keys\nGoal {_collectedKeys.Count} / 3");
    }
    
    /// <summary>
    /// ハッシュセットに入手したキーを追加する
    /// </summary>
    public void AddKey(string keyName)
    {
        _collectedKeys.Add(keyName);
        _combat._uiManager.UpdateQuestText(
            $"Collect the keys\nGoal {_collectedKeys.Count} / 3");

        if (HasAllKeys())
        {
            QuestFinished();
        }
    }

    /// <summary>
    /// 3つキーが揃ったら呼び出す
    /// </summary>
    private void QuestFinished()
    {
        _combat._uiManager.UpdateQuestText($"Head to the boss door.");
    }

    /// <summary>
    /// 3つキーが揃っていたらtrueを返す
    /// </summary>
    public bool HasAllKeys()
    {
        return _collectedKeys.Count >= 3;
    }

    public void UseKey()
    {
        _collectedKeys.Clear();
        _combat._uiManager.UpdateQuestText($"Fight the boss.");
    }
}