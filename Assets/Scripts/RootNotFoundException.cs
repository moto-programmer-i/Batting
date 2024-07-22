using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 解だせなかったときの例外
/// </summary>
public class RootNotFoundException : Exception
{
    public RootNotFoundException(double lowerBound, double upperBound, double fx)
        : base(FormatMessage(lowerBound, upperBound, fx))
    {
    }

    // 参考 https://stackoverflow.com/a/17831521
    public static string FormatMessage(double lowerBound, double upperBound, double fx)
    {
        return $"解がだせませんでした。lowerBound:{lowerBound} upperBound:{upperBound} 最終fx:{fx}";
    }
    
}
