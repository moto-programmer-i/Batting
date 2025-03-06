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
    /// 傾きを設定
    /// </summary>
    /// <param name="to">nullの場合は何もしない</param>
    public void SetSlope(Vector2Ex to)
    {
        if(to == null || to.Position == null) {
            return;
        }
        Slope = CalcSlope(to.Position, this.Position);
    }

    /// <summary>
    /// 傾きを設定
    /// </summary>
    /// <param name="list"></param>
    public static void SetSlope(List<Vector2Ex> list)
    {
        // １つ後の要素から傾きを決定
        for (int i = 0, next = 1; next < list.Count; ++i,++next) {
            list[i].SetSlope(list[next]);
        }
    }

    /// <summary>
    /// xの降順にソート
    /// </summary>
    /// <param name="list"></param>
    public static void SortDesc(List<Vector2Ex> list)
    {
        list.Sort((a, b) => MathF.Sign(b.Position.x - a.Position.x));
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

    public override bool Equals(object obj)
    {
        // 型が違ければfalse
        Vector2Ex v = obj as Vector2Ex;
        if (v == null) {
            return false;
        }

        // null同士か、値が同じならtrue
        if (Position == null && v.Position == null) {
            if (Slope == null && v.Slope == null) {
                return true;
            }
            return Slope == v.Slope;
        }

        return Position.Equals(v.Position) && Slope == v.Slope;
    }

    public override int GetHashCode()
    {
        // 参考 https://learn.microsoft.com/ja-jp/dotnet/fundamentals/runtime-libraries/system-object-gethashcode#code-try-1
        int hash = Position != null ? Position.GetHashCode() : 0;
        hash ^= Slope != null ? Slope.GetHashCode() : 0;
        return hash;
    }
}
