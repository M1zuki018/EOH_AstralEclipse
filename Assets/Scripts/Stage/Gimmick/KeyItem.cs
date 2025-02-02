using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayerSystem.Fight;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// ボスがいる部屋を開くためのキーアイテム
/// </summary>
public class KeyItem : InteractableItemBase
{
    [SerializeField] private string _keyName;
    [SerializeField] private float _moveY = 1.5f;
    private Transform _keyObject; //親オブジェクトのトランスフォーム
    
    [SerializeField] private List<Health> _targets = new List<Health>();
    
    private void Start()
    {
        _keyObject = transform.parent;
        _keyObject.DOMoveY(_keyObject.position.y + _moveY, 1.5f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    }
    
    public override async void Interact()
    {
        foreach (IHealth target in _targets)
        {
            if (!target.IsDead)
            {
                UIManager.Instance.ShowQuestMessage(); //警告メッセージを表示
                
                await UniTask.DelayFrame(100);
                
                UIManager.Instance.HideQuestMessage(); //警告メッセージを非表示
                return;
            }
        }
        
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
