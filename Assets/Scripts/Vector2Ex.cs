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
        
        return new Vector2Ex(v2, CalcSlope(v2, previous.Position));
    }

    /// <summary>
    /// 傾きを計算
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns>計算できない場合はnull</returns>
    public static float? CalcSlope(Vector2 v1, Vector2 v2)
    {
        if (v1.x == v2.x) {
            return null;
        }
        return (v2.y - v1.y) / (v2.x - v1.x);
    }

    /// <summary>
    /// マンハッタン距離を返す
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static float ManhattanDistance(Vector2Ex v1, Vector2Ex v2) {
        return Math.Abs(v1.Position.x - v2.Position.x) + Math.Abs(v1.Position.y - v2.Position.y);
    }

    public override string ToString()
    {
        return $"({Position.x}, {Position.y}), 傾き：{Slope}";
    }
}
