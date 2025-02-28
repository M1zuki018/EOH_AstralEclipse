using Cysharp.Threading.Tasks;
using PlayerSystem.ActionFunction;
using PlayerSystem.Movement;

/// <summary>
/// ステートマシンと各アクションをつなぐクラス
/// </summary>
public class PlayerActionHandler
{
    private readonly IMovable _mover; // 移動
    private readonly IJumpable _jumper;　// ジャンプ
    private readonly ISteppable _stepper; // ステップ
    private readonly IGuardable _gauder; // ガード
    private readonly IAttack _attack; // 通常攻撃
    private readonly ISkill _skill; // スキル
    private readonly ICounter _counter;

    public PlayerActionHandler(IMovable mover, IJumpable jumper,ISteppable steppable, 
        IGuardable gauder, IAttack attack, ISkill skill, ICounter counter)
    {
        _mover = mover;
        _jumper = jumper;
        _stepper = steppable;
        _gauder = gauder;
        _attack = attack;
        _skill = skill;
        _counter = counter;
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
    public void Step() => _stepper.Step();

    /// <summary>ガードを始める処理</summary>
    public void GuardStart() => _gauder.GuardStart();
    
    /// <summary>ガード入力処理</summary>
    public void Guard() => _gauder.Guard();

    /// <summary>ガードをやめる処理</summary>
    public void GuardEnd() => _gauder.GuardEnd();
    
    /// <summary>通常攻撃の入力処理</summary>
    public void Attack() => _attack.Attack();

    /// <summary>スキルが発動できるかチェックする</summary>
    public bool CanUseSkill => _skill.CanUseSkill();
    
    /// <summary>スキル攻撃処理</summary>
    public UniTask Skill() => _skill.UseSkill();

    /// <summary>カウンター時間の処理</summary>
    public void Counter() => _counter.CounterTask();
}
