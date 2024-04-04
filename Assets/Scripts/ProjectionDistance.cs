using System;
using MathNet.Numerics.RootFinding;
using UnityEngine;

/// <summary>
/// 射法投射の距離
/// 参考 https://moto-programmer-i-unity.blogspot.com/2023/12/tbd.html
/// </summary>
public class ProjectionDistance
{
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

    

    /// <summary>
    /// 物体の飛距離を計算
    /// </summary>
    /// <param name="m">質量</param>
    /// <param name="v0">初速</param>
    /// <param name="theta">角度（ラジアン）</param>
    /// <param name="k">空気抵抗</param>
    /// <param name="g">重力加速度</param>
    public ProjectionDistance(double m, double v0, double theta, double k, double g, double accuracy)
    {
        // 係数を計算して保存しておく
        k_mv0cos = k / (m * v0 * Math.Cos(theta));
        mv0cos_k = m * v0 * Math.Cos(theta) / k;
        onePluskv0sin_mg = 1 + (k * v0 * Math.Sin(theta) / (m * g));
        onePluskv0sin_mgXk_mv0cos = onePluskv0sin_mg * k_mv0cos;

        // 最低値は0だと解になってしまうため、少しずらす
        // （どれくらいずらすべきかは不明、今はとりあえず精度の3倍）
        lowerBound = 3 * accuracy;

        // Math.NETのFindRootのaccuracyがfxと比較してしまうため。他に良い対策があるかも
        // (本当はxをaccuracy以下の精度でだしたい)
        accuracy = F(accuracy);

        // 最大値は t -> ∞ のときのx
        // 参考 https://moto-programmer-i-unity.blogspot.com/2023/12/tbd.html#upperBound
        upperBound = m * v0 * Math.Cos(theta) / k;    

        distance = RobustNewtonRaphson.FindRoot(F, Df, lowerBound, upperBound, accuracy);
    }

    /// <summary>
    /// f(x)
    /// </summary>
    /// <param name="x"></param>
    /// <returns>-mv0cosθ/k + (mv0cosθ/k - x) e^{(1 + kv0sinθ/mg)kx/mv0cosθ}</returns>
    public double F(double x)
    {
        // return -mv0cos_k + (mv0cos_k - x) * Math.Exp(onePluskv0sin_mgXk_mv0cos * x);
        double fx = -mv0cos_k + (mv0cos_k - x) * Math.Exp(onePluskv0sin_mgXk_mv0cos * x);
        Debug.Log("fx " + fx);
        return fx;
    }

    /// <summary>
    /// df(x)
    /// </summary>
    /// <param name="x"></param>
    /// <returns>(-1 + (1 + kv0sinθ/mg) (1 - kx/mv0cosθ)) e^{(1 + kv0sinθ/mg)kx/mv0cosθ}</returns>
    public double Df(double x)
    {
        // return (-1 + onePluskv0sin_mg * (1 - k_mv0cos * x))
        //  * Math.Exp(onePluskv0sin_mgXk_mv0cos * x);
        double dfx = (-1 + onePluskv0sin_mg * (1 - k_mv0cos * x))
         * Math.Exp(onePluskv0sin_mgXk_mv0cos * x);
        Debug.Log("dfx " + dfx);
        return dfx;
    }

    public double GetDistance()
    {
        return distance;
    }
}
