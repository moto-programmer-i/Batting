using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://numerics.mathdotnet.com/api/MathNet.Numerics.RootFinding/Bisection.htm
// が希望通りに動作しないため自作。よい子は真似しないこと。

/// <summary>
/// 二分法 （参考 https://risalc.info/src/bisection-method.html ）により近似解を求めるクラス
/// </summary>
public static class Bisection
{
    /// <summary>
    /// デフォルトの精度
    /// </summary>
    public const double DEFAULT_ACCURACY = 0.1;

    // doubleのビット数64より大きい数字にとりあえずした
    /// <summary>
    /// デフォルトの計算回数
    /// </summary>
    public const int DEFAULT_MAX_ITERATIONS = 100;

    /// <summary>
    /// 二分法による解の近似値を求める。
    /// </summary>
    /// <param name="f">f(x)</param>
    /// <param name="lowerBound">解の最低値</param>
    /// <param name="upperBound">解の最大値</param>
    /// <param name="accuracy">精度（既存ライブラリとは違い、解の精度）</param>
    /// <param name="maxIterations">最大計算回数</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="RootNotFoundException"></exception>
    public static double FindRoot(Func<double, double> f, double lowerBound, double upperBound, double accuracy = DEFAULT_ACCURACY, int maxIterations = DEFAULT_MAX_ITERATIONS)
    {
        if (f == null) {
            throw new ArgumentException("式fがnullです");
        }
        if(lowerBound > upperBound) {
            throw new ArgumentException("lowerBound:" + lowerBound + " <= " + "upperBound:" + upperBound + " にしてください");
        }
        if(accuracy <= 0) {
            throw new ArgumentException("accuracy:" + accuracy + " は0より大きい値にしてください");
        }
        if(maxIterations <= 0) {
            throw new ArgumentException("accuracy:" + accuracy + " は0より大きい値にしてください");
        }

        
        double mid, range;
        double fMid = 0;
        double fUpperBound = f(upperBound);
        for(int i = 0; i < maxIterations; ++i) {
            // 中間点作成
            mid = (lowerBound + upperBound) / 2;
            fMid = f(mid);

            // 奇跡的にちょうど0なら解を返す
            if(fMid == 0) {
                return mid;
            }

            // 符号が同じならば、解 < 中間点
            if (fMid * fUpperBound > 0) {
                upperBound = mid;
                fUpperBound = fMid;
            }
            // 符号が違うならば、中間点 < 解
            else {
                lowerBound = mid;
            }

            // 解の範囲が精度以下ならば、中間の値を返す
            range = Math.Abs(upperBound - lowerBound);
            if(range <= accuracy) {
                return lowerBound + range / 2;
            }
        }

        // 解がだせなかったので例外
        throw new RootNotFoundException(lowerBound, upperBound, fMid);
    }
}
