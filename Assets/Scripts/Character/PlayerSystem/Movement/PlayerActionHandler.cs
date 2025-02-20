using System;
using System.Collections;
using System.Collections.Generic;
using PlayerSystem.ActionFunction;
using PlayerSystem.Movement;
using UnityEngine;

/// <summary>
/// ステートマシンと各アクションをつなぐクラス
/// </summary>
public class PlayerActionHandler
{
    private readonly IMovable _mover; //移動
    private readonly IJumpable _jumper;　//ジャンプ
    private readonly IWalkable _walker; //歩きと走りの切り替え
    private readonly ISteppable _stepper; //ステップ
    private readonly IGaudeable _gauder; //ガード
    private readonly ILockOnable _locker; //ロックオン
    private readonly PlayerCombat _combat; //アクション

    public PlayerActionHandler(IMovable mover, IJumpable jumper, 
        IWalkable walker, ISteppable steppable, IGaudeable gauder, ILockOnable locker, PlayerCombat combat)
    {
        _mover = mover;
        _jumper = jumper;
        _walker = walker;
        _stepper = steppable;
        _gauder = gauder;
        _locker = locker;
        _combat = combat;
    }
    
    // //TODO: 不具合の原因が解明出来たらここから呼び出すことになるかも
    // /// <summary>移動入力処理</summary>
    // public void Move() => _mover.Move();
    
    /// <summary>ジャンプ入力処理</summary>
    public void Jump() => _jumper.Jump();
    
    /// <summary>ポーズ入力処理</summary>
    public void Pause()
    {
        //TODO: ポーズ機能の実装を書く
        throw new System.NotImplementedException();
    }

    /// <summary>ステップ入力処理</summary>
    public void Step() => _stepper.TryUseStep();

    /// <summary>ガード入力処理</summary>
    public void Guard(bool input) => _gauder.Gaude(input);

    /// <summary>ロックオン入力処理</summary>
    public void LockOn() => _locker.LockOn();

    /// <summary>通常攻撃の入力処理</summary>
    public void Attack() => _combat.Attack();

    /// <summary>スキル攻撃の入力処理</summary>
    public void Skill(int index) => _combat.UseSkill(index);
}
