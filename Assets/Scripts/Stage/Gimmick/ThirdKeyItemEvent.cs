using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    [SerializeField] private List<EnemyBrain> _enemies = new List<EnemyBrain>();

    private void Start()
    {
        foreach (var enemy in _enemies)
        {
            enemy.EnemyMovement.enabled = false; //敵が動かないようにする   
            enemy.EnemyMovement.Agent.enabled = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //コライダーに接触したのがプレイヤーで、かつ既に2つキーを獲得している場合にイベント発生
        if (other.CompareTag("Player") && _inventory.CurrentHasKeys() == 2)
        {
            Event();
        }
    }

    private async void Event()
    {
        foreach (var enemy in _enemies)
        {
            enemy.EnemyMovement.enabled = true; //敵を動かし始める
            enemy.EnemyMovement.Agent.enabled = true;
        }
        
        _playerInput.DeactivateInput(); //プレイヤーの動きを止める
        UIManager.Instance.HidePlayerBattleUI(); //UIを隠す
        UIManager.Instance.HideRightUI();
        
        CameraManager.Instance.UseCamera(2);

        await UniTask.Delay(5000);
        
        CameraManager.Instance.UseCamera(0); //元のカメラに戻す
        UIManager.Instance.ShowRightUI(); //UIを戻す
        _playerInput.ActivateInput(); //操作できるようにする
    }

    #region デバッグ用

    [ContextMenu("Debug")]
    public void Debug()
    {
        _inventory.AddKey("test");
        _inventory.AddKey("test2");
    }

    #endregion
}
