using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ボスの前の扉。キーアイテムが3つ揃った状態でインタラクトしたら開く
/// </summary>
public class Door : InteractableItemBase
{
    public event Action OnDoorOpened;
    private Inventory _inventory;
    [SerializeField] private EnemyBrain _bossEnemyBrain;

    protected override void Start()
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
        Destroy(gameObject.transform.parent.gameObject);
        _inventory.UseKey(); //目標更新
        _player.GetComponent<PlayerBrain>().BB.IsBossBattle = true;
        ReadyForBattleChecker battleChecker = _player.GetComponentInChildren<ReadyForBattleChecker>();
        AudioManager.Instance.FadeIn(AudioType.BGM); //フェードイン
        AudioManager.Instance.ClipChange(AudioType.BGM, 1);
        battleChecker.StartBossBattle(_bossEnemyBrain); //ボス戦開始のイベント発火
        BossMover bossMover = _bossEnemyBrain.gameObject.GetComponent<BossMover>();
        bossMover.BattleStart().Forget();
    }
}
