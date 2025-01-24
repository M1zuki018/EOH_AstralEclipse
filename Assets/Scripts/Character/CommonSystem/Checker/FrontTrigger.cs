using UnityEngine;

/// <summary>
/// 前方の判定に使用するTriggerを管理するスクリプト
/// </summary>
public class FrontTrigger : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("VaultObject"))
        {
            _playerMovement.PlayerState.CanVault = true; //ヴォルトアクションを使用可能にする
            _playerMovement._valutTargetObjects.Add(other.gameObject.transform);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            _playerMovement.PlayerState.CanClimb = true;
            _playerMovement.PlayerState.WallNormal = other.transform.forward;
        }
        else if (other.gameObject.CompareTag("Interactable")) //インタラクトできるオブジェクトが範囲内にあったら
        {
            _playerMovement.InteractableItem = other.transform.GetComponentInChildren<InteractableItemBase>(); //インタラクトできるオブジェクトの情報を渡す
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("VaultObject"))
        {
            _playerMovement.PlayerState.CanVault = false; //使用不能にする
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            _playerMovement.PlayerState.CanClimb = false;
        }
        else if (other.gameObject.CompareTag("Interactable")) //インタラクトできるオブジェクトが範囲内から出たら
        {
            _playerMovement.InteractableItem = null; //情報を削除する
        }
    }
}
