using PlayerSystem.Input;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerのInputSystemを管理するクラス
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    private IPlayerInputReceiver _playerInputReceiver; //入力情報

    private void Start()
    {
        _playerInputReceiver = GetComponent<PlayerMovement>().PlayerInputReceiver;
    }
    
    #region 入力されたときのメソッド一覧
    
    /// <summary>攻撃処理</summary>
    public void OnAttack(InputAction.CallbackContext context) => HandleAttackInput(context);

    /// <summary>スキル処理</summary>
    public void OnSkill1(InputAction.CallbackContext context) => HandleSkillInput(context, 1); 
    public void OnSkill2(InputAction.CallbackContext context) => HandleSkillInput(context, 2);
    public void OnSkill3(InputAction.CallbackContext context) => HandleSkillInput(context, 3);
    public void OnSkill4(InputAction.CallbackContext context) => HandleSkillInput(context, 4);
    
    /// <summary>移動処理</summary>
    public void OnMove(InputAction.CallbackContext context) => _playerInputReceiver.HandleMoveInput(context.ReadValue<Vector2>());

    /// <summary>ジャンプ処理</summary>
    public void OnJump(InputAction.CallbackContext context) => HandleJumpInput(context);
    
    /// <summary>歩きと走り状態を切り替える</summary>
    public void OnWalk(InputAction.CallbackContext context) => _playerInputReceiver.HandleWalkInput();
    
    /// <summary>ステップ</summary>
    public void OnStep(InputAction.CallbackContext context) => HandleStepInput(context);
    
    /// <summary>ガード状態を切り替える</summary>
    public void OnGuard(InputAction.CallbackContext context) => HandleGuardInput(context);

    /// <summary>ロックオン機能</summary>
    public void OnLockOn(InputAction.CallbackContext context) => HandleLockOnInput(context);

    public void OnPause(InputAction.CallbackContext context) => _playerInputReceiver.HandlePauseInput();
    
    public void OnNone(InputAction.CallbackContext context) => Debug.Log("登録されていないボタンです");

    #endregion
    
    #region 入力の条件文

    private void HandleAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed) _playerInputReceiver.HandleAttackInput();
    }
    
    private void HandleSkillInput(InputAction.CallbackContext context, int index)
    {
        //index で スキル1~4のどのボタンを押されたか判断する
        if (context.performed) _playerInputReceiver.HandleSkillInput(index);
    }
    private void HandleJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed) _playerInputReceiver.HandleJumpInput();
    }

    private void HandleStepInput(InputAction.CallbackContext context)
    {
        if (context.performed) _playerInputReceiver.HandleStepInput();
    }

    private void HandleLockOnInput(InputAction.CallbackContext context)
    {
        if (context.performed) _playerInputReceiver.HandleLockOnInput();
    }

    private void HandleGuardInput(InputAction.CallbackContext context)
    {
        if (context.performed) _playerInputReceiver.HandleGaudeInput(true);
        if (context.canceled) _playerInputReceiver.HandleGaudeInput(false);
    }

    #endregion
}
