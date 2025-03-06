using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberUtils
{
    /// <summary>
    /// NaNか判定する。nullの場合もtrue
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static bool IsNaN(float? f)
    {
        if(f == null) {
            return true;
        }
        return float.IsNaN((float)f);
    }
}
