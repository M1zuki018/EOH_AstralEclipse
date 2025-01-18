using UnityEngine;
/// <summary>
/// アニメーションテスト用のクラス
/// </summary>
public class Test : MonoBehaviour
{
    Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _animator.SetTrigger("Test");
        } 
    }
}
