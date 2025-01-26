using UnityEngine;

/// <summary>
/// BGM再生を管理する
/// </summary>
public class BGMHandler : MonoBehaviour
{
    [SerializeField] private Door _door;
    [SerializeField] private int _defaultBGMIndex = 0;
    private AudioType _myType = AudioType.BGM;

    private void Start()
    {
        _door.OnDoorOpened += HandleDoorOpened; //ボス戦突入時のイベントを登録
        
        AudioManager.Instance.ClipChange(_myType, _defaultBGMIndex); //BGMを初期化
        AudioManager.Instance.FadeIn(_myType);
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
        AudioManager.Instance.ClipChange(_myType, 1);
    }
}
