using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListUtils
{
    /// <summary>
    /// Listが空かどうか判定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns>true: nullまたは空、false: それ以外</returns>
    public static bool IsEmpty<T>(List<T> list)
    {
        return list == null || !list.Any();
    }
}
