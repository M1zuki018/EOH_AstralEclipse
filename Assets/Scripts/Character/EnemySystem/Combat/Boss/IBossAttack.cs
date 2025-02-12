using Cysharp.Threading.Tasks;

/// <summary>
/// ボスの攻撃を管理するクラスが継承するインターフェース
/// </summary>
public interface IBossAttack
{
    string AttackName { get; }
    UniTask Fire();
}