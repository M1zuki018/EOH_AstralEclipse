using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// 全ステートマシンのベースとなるクラス
/// </summary>
public abstract class BaseStateMachine<TEnum, TState> where TEnum : Enum where TState : IState 
{
    protected Dictionary<TEnum, TState> States = new();
    public ReactiveProperty<TEnum> CurrentState { get; private set; } = new();

    /// <summary>
    /// 初期化
    /// </summary>
    protected void Initialize(TEnum initialState) 
    {
        CurrentState.Value = initialState;
        States[initialState].Enter().Forget();
    }

    /// <summary>
    /// ステートを変更する
    /// </summary>
    public void ChangeState(TEnum newState)
    {
        if (!States.ContainsKey(newState)) return;

        States[CurrentState.Value].Exit().Forget();
        CurrentState.Value = newState;
        States[newState].Enter().Forget();
        Debug.Log($"<color=yellow>[ChangeState]</color>　ステートが変更されました : 現在 {newState}");
    }

    /// <summary>
    /// Executeメソッドを呼び続ける
    /// </summary>
    public void Update()
    {
        if (States.ContainsKey(CurrentState.Value)) 
        {
            States[CurrentState.Value].Execute().Forget();
        }
    }

    /// <summary>
    /// FixedExecuteメソッドを呼び続ける（特定のステートのみ）
    /// </summary>
    public void FixedUpdate()
    {
        if (States.ContainsKey(CurrentState.Value) && States[CurrentState.Value] is IFixedUpdateState fixedState) 
        {
            fixedState.FixedExecute().Forget();
        }
    }
}