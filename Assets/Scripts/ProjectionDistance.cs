using System;
using MathNet.Numerics;
using MathNet.Numerics.RootFinding;
using UnityEngine;

/// <summary>
/// 射法投射の距離
/// 参考 https://moto-programmer-i-unity.blogspot.com/2023/12/tbd.html
/// </summary>
public class ProjectionDistance
{
    /// <summary>
    /// 初期高さ
    /// </summary>
    private double h0;

    /// <summary>
    /// k/mv0cosθ
    /// </summary>
    private double k_mv0cos;

    /// <summary>
    /// mv0cosθ/k
    /// </summary>
    private double mv0cos_k;

    /// <summary>
    /// 1 + kv0sinθ/mg
    /// </summary>
    private double onePluskv0sin_mg;

    /// <summary>
    /// (1 + kv0sinθ/mg) * k/mv0cosθ
    /// </summary>
    private double onePluskv0sin_mgXk_mv0cos;

    private double distance;

    private double lowerBound;
    private double upperBound;

    private static double maxTheta = Math.PI / 2;
    private static double minTheta = -maxTheta;


    

    /// <summary>
    /// 物体の飛距離を計算
    /// </summary>
    /// <param name="m">質量</param>
    /// <param name="v0">初速</param>
    /// <param name="theta">角度（ラジアン）</param>
    /// <param name="k">空気抵抗</param>
    /// <param name="h0">初期高さ</param>
    /// <param name="g">重力加速度</param>
    /// <exception cref="NonConvergenceException">不正な値、または計算に失敗したとき</exception>
    public ProjectionDistance(double m, double v0, double theta, double k, double g, double h0, double accuracy)
    {
        this.h0 = h0;

        // 値チェック
        if (m <= 0) {
            throw new NonConvergenceException("不正な質量：" + m);
        }
        if (v0 <= 0) {
            throw new NonConvergenceException("不正な速度：" + v0);
        }
        if (theta >= maxTheta || theta <= minTheta) {
            throw new NonConvergenceException("不正な角度：" + maxTheta);
        }
        // 空気抵抗が0の場合の0除算回避
        // 参考 https://nekodamashi-math.blog.ss-blog.jp/2019-08-25
        if (k == 0) {
            // v0^2 sin2θ / g
            distance = v0 * v0 * Math.Sin(2 * theta) / g;
            return;
        }

        // gはUnity上マイナスのまま入る可能性があるので、便宜上+にする
        g = Math.Abs(g);
        
        // 係数を計算して保存しておく
        k_mv0cos = k / (m * v0 * Math.Cos(theta));
        mv0cos_k = m * v0 * Math.Cos(theta) / k;
        onePluskv0sin_mg = 1 + (k * v0 * Math.Sin(theta) / (m * g));
        onePluskv0sin_mgXk_mv0cos = onePluskv0sin_mg * k_mv0cos;

        // 最低値は0だと解になってしまうため、精度分ずらす
        lowerBound = accuracy;

        // Math.NETのFindRootのaccuracyがfxと比較してしまうため対策。他に良いのがあるかも
        // F(accuracy)だとlowerBoundで値が確定しまう可能性があるため、半分にする
        // (これでいいかは不明、本当はxをaccuracy以下の精度でだしたい)
        double fAccuracy = F(accuracy) / 2;

        // F(accuracy)がマイナスになる場合はそもそも小さすぎて飛距離をだせない
        if (fAccuracy < 0) {
            throw new NonConvergenceException("飛距離が精度の値" + accuracy + "未満のため、計算できません");
        }

        // 最大値は t -> ∞ のときのx
        // 参考 https://moto-programmer-i-unity.blogspot.com/2023/12/tbd.html#upperBound
        upperBound = m * v0 * Math.Cos(theta) / k;    

        distance = RobustNewtonRaphson.FindRoot(F, Df, lowerBound, upperBound, fAccuracy);
    }

    /// <summary>
    /// f(x)
    /// </summary>
    /// <param name="x"></param>
    /// <returns>-mv0cosθ/k + (mv0cosθ/k - x) e^{(1 + kv0sinθ/mg)kx/mv0cosθ}</returns>
    public double F(double x)
    {
        return -mv0cos_k + (mv0cos_k - x) * Math.Exp(onePluskv0sin_mgXk_mv0cos * x) + h0;
    }

    /// <summary>
    /// df(x)
    /// </summary>
    /// <param name="x"></param>
    /// <returns>(-1 + (1 + kv0sinθ/mg) (1 - kx/mv0cosθ)) e^{(1 + kv0sinθ/mg)kx/mv0cosθ}</returns>
    public double Df(double x)
    {
        return (-1 + onePluskv0sin_mg * (1 - k_mv0cos * x))
         * Math.Exp(onePluskv0sin_mgXk_mv0cos * x);
    }

    public double GetDistance()
    {
        return distance;
    }
}
