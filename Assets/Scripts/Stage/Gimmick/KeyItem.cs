using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// ボスがいる部屋を開くためのキーアイテム
/// </summary>
public class KeyItem : InteractableItemBase
{
    [SerializeField] private string _keyName;
    [SerializeField] private float _moveY = 1.5f;
    private Transform _keyObject; //親オブジェクトのトランスフォーム

    private void Start()
    {
        _keyObject = transform.parent;
        _keyObject.DOMoveY(_keyObject.position.y + _moveY, 1.5f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    }
    
    public override async void Interact()
    {
        //CameraManager.Instance.UseTargetGroup(_keyObject, 1, 0.5f);
        //CameraManager.Instance.UseCamera(1);
        
        AudioManager.Instance.PlaySE(6);
        UIManager.Instance.QuestUpdate();
        
        Inventory inventory = _player.GetComponent<Inventory>(); //プレイヤーからイベントリクラスを取得
        if (inventory != null)
        {
            inventory.AddKey(_keyName);
            Destroy(gameObject.transform.parent.gameObject); //キーを追加したらオブジェクトを削除する
        }
        
        //CameraManager.Instance.UseCamera(0);
    }
}
