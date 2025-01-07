using System;
using UnityEngine;

/// <summary>
/// 足元が地面/壁か判定する
/// </summary>
public class GroundTrigger : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            _playerMovement.IsGround = true;
        }
        else if (other.gameObject.tag == "Wall")
        {
            _playerMovement.IsWall = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            _playerMovement.IsGround = false;
        }
        else if (other.gameObject.tag == "Wall")
        {
            _playerMovement.IsWall = false;
        }
    }
}
