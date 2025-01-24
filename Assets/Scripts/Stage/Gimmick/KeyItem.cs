using UnityEngine;

/// <summary>
/// ボスがいる部屋を開くためのキーアイテム
/// </summary>
public class KeyItem : InteractableItemBase
{
    [SerializeField] private string _keyName;
    public override void Interact()
    {
        Inventory inventory = _player.GetComponent<Inventory>(); //プレイヤーからイベントリクラスを取得
        if (inventory != null)
        {
            inventory.AddKey(_keyName);
            Debug.Log($"{_keyName} をインベントリに追加しました");
            Destroy(gameObject.transform.parent.gameObject); //キーを追加したらオブジェクトを削除する
        }
    }
}
