using Cysharp.Threading.Tasks;

/// <summary>
/// 各ステートが継承するインターフェース
/// </summary>
public interface IState
{
    /// <summary>
    /// このステートになったときに呼ばれる
    /// </summary>
    UniTask Enter();

    /// <summary>
    /// このステート中はずっと呼ばれる
    /// </summary>
    UniTask Execute();
    
    /// <summary>
    /// このステートから変わるときに呼ばれる
    /// </summary>
    UniTask Exit();
}
