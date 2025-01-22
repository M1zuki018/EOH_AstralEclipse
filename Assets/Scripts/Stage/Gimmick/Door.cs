using System;
using UnityEngine;

/// <summary>
/// ボスの前の扉。キーアイテムが3つ揃った状態でインタラクトしたら開く
/// </summary>
public class Door : InteractableItemBase
{
    public event Action OnDoorOpened;  
    
    public override void Interact()
    {
        Inventory inventory = _player.GetComponent<Inventory>();
        if (inventory.HasAllKeys())
        {
            //キーが揃っていたらイベント発火
            OnDoorOpened?.Invoke();
            _collider.enabled = false; //判定を消す
            Debug.Log("Doorが開いた");
        }
        else
        {
            //キーが揃っていなかったら
            Debug.Log("キーが足りません");
        }
    }
}
