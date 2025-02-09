using System;
using UniRx;
using UnityEngine.InputSystem;

public static class InputActionExtensions
{
    /// <summary>
    /// InputSystemのInputActionを簡単にObservableに変換する拡張メソッド
    /// </summary>
    public static IObservable<InputAction.CallbackContext> PerformedAsObservable(this InputAction action)
    {
        return Observable.FromEvent<InputAction.CallbackContext>(
            h => action.performed += h,
            h => action.performed -= h
        );
    }
}