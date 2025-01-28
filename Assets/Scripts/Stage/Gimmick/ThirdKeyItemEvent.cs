using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 3つ目のキーアイテムの前で発生するイベントを管理するクラス
/// </summary>
public class ThirdKeyItemEvent : MonoBehaviour
{
    [Header("初期設定")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Inventory _inventory;

    private void OnTriggerEnter(Collider other)
    {
        //コライダーに接触したのがプレイヤーで、かつ既に2つキーを獲得している場合にイベント発生
        if (other.CompareTag("Player") && _inventory.CurrentHasKeys() == 2)
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

    #region デバッグ用

    [ContextMenu("Debug")]
    public void Debug()
    {
        _inventory.AddKey("test");
        _inventory.AddKey("test2");
    }

    #endregion
}
