/// <summary>
/// ボスがいる部屋を開くためのキーアイテム
/// </summary>
public class KeyItem : InteractableItemBase
{
    public override void Interact()
    {
        Inventory inventory = _player.GetComponent<Inventory>(); //プレイヤーからイベントリクラスを取得
        if (inventory != null)
        {
            inventory.AddKey(gameObject.name);
            Destroy(gameObject.transform.parent.gameObject); //キーを追加したらオブジェクトを削除する
        }
    }
}
