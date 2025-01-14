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
            _playerMovement.CanVault = true; //ヴォルトアクションを使用可能にする
            _playerMovement._valutTargetObjects.Add(other.gameObject.transform);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            _playerMovement.CanClimb = true;
            _playerMovement.WallNormal = other.transform.forward;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("VaultObject"))
        {
            _playerMovement.CanVault = false; //使用不能にする
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            _playerMovement.CanClimb = false;
        }
    }
}
