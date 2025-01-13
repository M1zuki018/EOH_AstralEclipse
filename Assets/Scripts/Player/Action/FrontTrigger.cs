using UnityEngine;

/// <summary>
/// 前方の判定に使用するTriggerを管理するスクリプト
/// </summary>
public class VaultTrigger : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("VaultObject"))
        {
            _playerMovement.IsVault = true; //使用可能にする
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("VaultObject"))
        {
            _playerMovement.IsVault = false; //使用不能にする
        }
    }
}
