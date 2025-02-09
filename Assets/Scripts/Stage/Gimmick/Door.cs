using System;
using UnityEngine;

/// <summary>
/// ボスの前の扉。キーアイテムが3つ揃った状態でインタラクトしたら開く
/// </summary>
public class Door : InteractableItemBase
{
    public event Action OnDoorOpened;
    private Inventory _inventory;
    [SerializeField] private EnemyBrain _bossEnemyBrain;

    private void Start()
    {
        OnDoorOpened += HandleDoorOpen; //イベント登録
    }

    private void OnDestroy()
    {
        OnDoorOpened -= HandleDoorOpen; //イベント解除
    }
    
    public override void Interact()
    {
        if (_player != null)
        {
            _player.TryGetComponent(out Inventory inventory);
            _inventory = inventory;
            
            if (_inventory.HasAllKeys())
            {
                //キーが揃っていたらイベント発火
                OnDoorOpened?.Invoke();
            }
            else
            {
                //キーが揃っていなかったら
                Debug.Log("キーが足りません");
            }
        }
    }

    /// <summary>
    /// ドアを開く
    /// </summary>
    private void HandleDoorOpen()
    {
        //_collider.enabled = false; //判定を消す
        Destroy(gameObject.transform.parent.gameObject); //TODO:処理を書く
        _inventory.UseKey(); //目標更新
        _player.TryGetComponent(out PlayerMovement playerMovement);
        playerMovement.PlayerState.IsBossBattle = true; //ステートを更新
        ReadyForBattleChecker battleChecker = _player.GetComponentInChildren<ReadyForBattleChecker>();
        battleChecker.StartBossBattle(_bossEnemyBrain); //ボス戦開始のイベント発火
        BossMover bossMover = _bossEnemyBrain.gameObject.GetComponent<BossMover>();
        bossMover.BattleStart();
    }
}
