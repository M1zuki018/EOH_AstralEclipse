using UnityEngine;

/// <summary>
/// BGM再生を管理する
/// </summary>
public class BGMHandler : MonoBehaviour
{
    [SerializeField] private AudioManager _manager;
    [SerializeField] private Door _door;
    [SerializeField] private int _defaultBGMIndex = 0;
    private AudioType _myType = AudioType.BGM;

    private void Start()
    {
        _door.OnDoorOpened += HandleDoorOpened; //ボス戦突入時のイベントを登録
        
        _manager.ClipChange(_myType, _defaultBGMIndex); //BGMを初期化
    }

    private void OnDestroy()
    {
        _door.OnDoorOpened -= HandleDoorOpened;
    }

    /// <summary>
    /// ボス戦突入時のBGM変更
    /// </summary>
    private void HandleDoorOpened()
    { 
        _manager.ClipChange(_myType, 1);
    }
}
