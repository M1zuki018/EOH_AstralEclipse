using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// パリィ後など、カウンター時間中の処理
/// </summary>
public interface ICounter
{
    UniTask CounterTask();
}