using Cysharp.Threading.Tasks;

/// <summary>
/// FixedUpdateが必要なステートのみ実装するインターフェース
/// </summary>
public interface IFixedUpdateState
{
    UniTask FixedExecute();
}