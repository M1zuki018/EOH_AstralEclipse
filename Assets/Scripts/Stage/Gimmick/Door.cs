using System;
using UnityEngine;

/// <summary>
/// ボスの前の扉。キーアイテムが3つ揃った状態でインタラクトしたら開く
/// </summary>
public class Door : InteractableItemBase
{
    public event Action OnDoorOpened;

    private void Start()
    {
        OnDoorOpened += HandleDoorOpen; //イベント登録
    }

    private void OnDestroy()
    {
        OnDoorOpened -= HandleDoorOpen; //イベント解除
    }
    
    public override void Interact()
    {
        Inventory inventory = _player.GetComponent<Inventory>();
        
        if (inventory.HasAllKeys())
        {
            //キーが揃っていたらイベント発火
            OnDoorOpened?.Invoke();
        }
        else
        {
            //キーが揃っていなかったら
            Debug.Log("キーが足りません");
        }
    }

    /// <summary>
    /// ドアを開く
    /// </summary>
    private void HandleDoorOpen()
    {
        //_collider.enabled = false; //判定を消す
        Destroy(gameObject.transform.parent.gameObject); //TODO:処理を書く
        Debug.Log("Doorが開いた");
    }
}
