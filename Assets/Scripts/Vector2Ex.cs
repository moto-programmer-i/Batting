using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 座標 + αの情報を持つクラス
/// </summary>
public class Vector2Ex
{
    public Vector2 Position {get; set;}

    /// <summary>
    /// 傾き
    /// </summary>
    public Nullable<float> Slope {get; set;}

    public Vector2Ex(){}

    public Vector2Ex(Vector2 position, Nullable<float> slope)
    {
        this.Position = position;
        this.Slope = slope;
    }

    /// <summary>
    /// Vector2 を変換
    /// </summary>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vector2Ex From(Vector2 v2)
    {
        return new Vector2Ex(v2, null);
    }

    /// <summary>
    /// Vector2と前回の点から変換
    /// </summary>
    /// <param name="v2"></param>
    /// <param name="previous"></param>
    /// <returns></returns>
    public static Vector2Ex From(Vector2 v2, Vector2Ex previous)
    {
        if (previous == null) {
            return From(v2);
        }
        
        return new Vector2Ex(v2, (v2.y - previous.Position.y) / (v2.x - previous.Position.x));
    }
}
