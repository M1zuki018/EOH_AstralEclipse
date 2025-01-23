using UnityEngine;

/// <summary>
/// インタラクト可能なアイテムの基底クラス
/// </summary>
public abstract class InteractableItemBase : MonoBehaviour
{
    protected Collider _collider;
    protected GameObject _player;

    /// <summary>
    /// インタラクトできるかどうか
    /// Playerタグがついたオブジェクトが範囲内にいること
    /// 臨戦状態でないことが条件
    /// </summary>
    public bool CanGet { get; protected set; }

    protected virtual void Start()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CanGet = true;
            _player = other.gameObject;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CanGet = false;
        }
    }

    /// <summary>
    /// インタラクトしたときの動作（各アイテムごとに実装）
    /// TODO: インタラクトはこのメソッドを呼び出す
    /// </summary>
    public abstract void Interact();

    public InteractableItemBase GetInteractableItem() => this;
}