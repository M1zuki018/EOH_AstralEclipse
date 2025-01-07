using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Vector3 _playerPosition;

    private void Start()
    {
        _playerPosition = transform.position;
    }

    void Update()
    {
        if (transform.position.y < -4f)
        {
            transform.position = _playerPosition;
        }
    }
}
