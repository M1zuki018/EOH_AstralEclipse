/// <summary>
/// 各ステートが継承するインターフェース
/// </summary>
public interface IState
{
    /// <summary>
    /// このステートになったときに呼ばれる
    /// </summary>
    void Enter();

    /// <summary>
    /// このステート中はずっと呼ばれる
    /// </summary>
    void Execute();
    
    /// <summary>
    /// このステートから変わるときに呼ばれる
    /// </summary>
    void Exit();
}
