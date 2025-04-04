using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 非同期の処理 UniTaskが良さそうだが、将来のバージョンで代替がでるらしいので見送り
/// </summary>
public static class AsyncUtils
{
    public static void Delay(MonoBehaviour instance, float seconds, Action action)
    {
        instance.StartCoroutine(Wait(seconds, action));
        
    }

    private static IEnumerator Wait(float seconds, Action action)
    {
        yield return new WaitForSecondsRealtime(seconds);
        action.Invoke();
    }

}
