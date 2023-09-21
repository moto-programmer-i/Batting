using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtils
{
    /// <summary>
    /// 補間する点の数
    /// </summary>
    // readonlyだと引数の初期値に使えなかった
    public const int DEFAULT_LERP_PRECISION = 5;

    /// <summary>
    /// Vector2.Lerpのtの最大値
    /// </summary>
    public static readonly float LERP_MAX_RANGE = 1;

    /// <summary>
    /// 2点を補間する点を使う
    /// </summary>
    /// <param name="start"></param>
    /// <param name="destination"></param>
    /// <param name="precision"></param>
    /// <param name="midPointHandler"></param>
    public static void withLerpPoints(Vector2 start, Vector2 destination, Action<Vector2> midPointHandler, int precision = DEFAULT_LERP_PRECISION)
    {
        // deltaずつ補間する
        float delta = LERP_MAX_RANGE / precision;
        for(float i = 0; i < precision; ++i) {
            midPointHandler(Vector2.Lerp(start, destination, i * delta));
        }
    }
}
