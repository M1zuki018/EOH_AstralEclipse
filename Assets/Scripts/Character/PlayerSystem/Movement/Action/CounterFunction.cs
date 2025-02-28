using Cysharp.Threading.Tasks;
using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// カウンター中の処理
/// </summary>
public class CounterFunction : ICounter
{
    private PlayerBlackBoard _bb;

    public CounterFunction(PlayerBlackBoard bb)
    {
        _bb = bb;
    }
    
    public async UniTask CounterTask()
    {
        Debug.Log("Counter時間");
        
        await UniTask.Delay((int)_bb.Status.CounterTime * 1000);
        
        Debug.Log("Counter時間終了");
    }
}
