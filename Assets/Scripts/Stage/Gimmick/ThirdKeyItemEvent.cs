using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 3つ目のキーアイテムの前で発生するイベントを管理するクラス
/// </summary>
public class ThirdKeyItemEvent : MonoBehaviour
{
    [Header("初期設定")]
    [SerializeField] private PlayerInput _playerInput;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Event();
        }
    }

    private void Event()
    {
        _playerInput.DeactivateInput(); //プレイヤーの動きを止める
        UIManager.Instance.HidePlayerBattleUI(); //バトルUIを隠す
        
        CameraManager.Instance.UseCamera(2);
    }
    //プレイヤーの動きを止める
    //カメラを切り替える
    //奥からエネミーが歩いてくる
    //カメラを切り替える
    //操作できるようにする
}
