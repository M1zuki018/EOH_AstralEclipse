using UnityEngine;

/// <summary>
/// ボスがいる部屋を開くためのキーアイテム
/// </summary>
public class KeyItem : MonoBehaviour
{
    //敵がいない状態で
    //インタラクトされると開く
    private Collider _collider;
    public bool CanGet{ get; private set; } //入手できるかどうか
    
    private void Start()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO: この条件に「臨戦状態でない場合」というのも足す
        if (other.CompareTag("Player"))
        {
            //プレイヤータグのついたオブジェクトが衝突したら、獲得可能状態にする
            CanGet = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //プレイヤータグのついたオブジェクトが離れたら、獲得不可能状態に戻す
            CanGet = false;
        }
    }
}
