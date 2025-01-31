using System.Collections.Generic;
using UnityEngine;

public class DebugSystem : MonoBehaviour
{
    [SerializeField] private DebugMode _debugMode;
    [SerializeField] private bool _skipOpening;
    [SerializeField] private List<EnemyBrain> _enemies = new List<EnemyBrain>();
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Door _door;
    [SerializeField] private Transform _player;
    [SerializeField] private List<Vector3> _startPoints = new List<Vector3>();
    
    
    private Animator _playerAnimator;
    private CharacterController _playerController;
    
    private void Start()
    {
        _playerAnimator = _player.GetComponent<Animator>();
        _playerController = _player.GetComponent<CharacterController>();

        switch (_debugMode)
        {
            case DebugMode.FirstBattle:
                StartFirstBattle();
                break;
            case DebugMode.SecondBattle:
                StartSecondBattle();
                break;
            case DebugMode.ThirdBattle:
                StartThirdBattle();
                break;
            case DebugMode.DoorCheck:
                DooCheck();
                break;
            case DebugMode.BossBattle:
                StartBossBattle();
                break;
            case DebugMode.OnlyBossTest:
                OnlyBossTest();
                break;
            case DebugMode.ActionTest:
                ActionTest();
                break;
            default:
                Debug.Log("Start");
                break;
        }
    }
    
    [ContextMenu("StartFirstBattle")]
    public void StartFirstBattle()
    {
        PlayerCompulsionMove(0);
    }

    [ContextMenu("StartSecondBattle")]
    public void StartSecondBattle()
    {
        PlayerCompulsionMove(1);
        
        for (int i = 0; i < 2; i++)
        {
            _enemies[i]?.Debug_EnemyDeath();
        }
        
        _inventory.AddKey("test");
    }

    [ContextMenu("StartThirdBattle")]
    public void StartThirdBattle()
    {
        PlayerCompulsionMove(2);
        
        for (int i = 0; i < 6; i++)
        {
            _enemies[i]?.Debug_EnemyDeath();
        }
        
        _inventory.AddKey("test");
        _inventory.AddKey("test2");
    }

    [ContextMenu("DoorCheck")]
    public void DooCheck()
    {
        PlayerCompulsionMove(3);
        
        for (int i = 0; i < 9; i++)
        {
            _enemies[i]?.Debug_EnemyDeath();
        }
        
        _inventory.AddKey("test");
        _inventory.AddKey("test2");
        _inventory.AddKey("test3");
    }
    
    [ContextMenu("StartBossBattle")]
    public void StartBossBattle()
    {
        PlayerCompulsionMove(4);
        
        for (int i = 0; i < 9; i++)
        {
            _enemies[i]?.Debug_EnemyDeath();
        }
        
        _inventory.AddKey("test");
        _inventory.AddKey("test2");
        _inventory.AddKey("test3");
        
        _door.Interact();
    }

    [ContextMenu("OnlyBossTest")]
    public void OnlyBossTest()
    {
        for (int i = 0; i < 9; i++)
        {
            _enemies[i]?.Debug_EnemyDeath();
        }
        
        _inventory.AddKey("test");
        _inventory.AddKey("test2");
        _inventory.AddKey("test3");
        
        _door.Interact();
    }

    private void ActionTest()
    {
        PlayerMovement playerMovement = _player.gameObject.GetComponent<PlayerMovement>();
        playerMovement.PlayerState.DebugMode = true;
    }

    /// <summary>
    /// プレイヤーの位置を強制変更
    /// </summary>
    private void PlayerCompulsionMove(int index)
    {
        _playerAnimator.applyRootMotion = false;
        _playerController.Move(_startPoints[index]);
        _playerAnimator.applyRootMotion = true;
    }
}

public enum DebugMode
{
    None,
    FirstBattle,
    SecondBattle,
    ThirdBattle,
    DoorCheck,
    BossBattle,
    OnlyBossTest,
    ActionTest,
}
