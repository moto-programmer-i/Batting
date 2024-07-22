using System;

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
    private double y0;

    /// <summary>
    /// xの係数の逆数 
    /// </summary>
    private double reciprocalOfCoefficientOfX;

    /// <summary>
    /// xの係数 
    /// </summary>
    private double coefficient2OfX;

    /// <summary>
    /// 1 - kdt
    /// </summary>
    private double one_kdt;

    /// <summary>
    /// y0とか
    /// </summary>
    private double y0Term;

    

    private double distance;

    private double lowerBound;
    private double upperBound;

    private static double maxTheta = Math.PI / 2;
    private static double minTheta = -maxTheta;


    

    /// <summary>
    /// 物体の飛距離を計算
    /// </summary>
    /// <param name="dt">（UnityではFixed Timestep）</param>
    /// <param name="v0">初速</param>
    /// <param name="theta">角度（ラジアン）</param>
    /// <param name="k">空気抵抗</param>
    /// <param name="g">重力加速度</param>
    /// <param name="y0">初期高さ</param>
    /// <param name="accuracy">精度</param>
    /// <exception cref="ArgumentException">不正な引数の場合</exception>
    public ProjectionDistance(double dt, double v0, double theta, double k, double g, double y0, double accuracy = Bisection.DEFAULT_ACCURACY)
    {
        this.y0 = y0;

        // 値チェック
        if (dt <= 0) {
            throw new ArgumentException("不正なdt：" + dt);
        }
        if (v0 <= 0) {
            throw new ArgumentException("不正な速度：" + v0);
        }
        if (theta >= maxTheta || theta <= minTheta) {
            throw new ArgumentException("不正な角度：" + maxTheta);
        }
        // 空気抵抗が0の場合の0除算回避
        // 参考 https://nekodamashi-math.blog.ss-blog.jp/2019-08-25
        if (k == 0) {
            // v0^2 sin2θ / g
            distance = v0 * v0 * Math.Sin(2 * theta) / g;
            return;
        }

        // gは計算上、必ず負
        g = -Math.Abs(g);
        
        // 係数を計算して保存しておく
        one_kdt = 1 - k * dt;
        double gdt = g * dt;
        double vx0 = v0 * Math.Cos(theta);
        double vy0 = v0 * Math.Sin(theta);
        reciprocalOfCoefficientOfX = vx0 * one_kdt / k;
        coefficient2OfX = (vy0 - g * one_kdt/k) * k / (gdt * vx0 * one_kdt);
        y0Term = k * y0 / (gdt * one_kdt);

        // 最大値は n -> ∞ のときのx
        // 参考 https://moto-programmer-i-unity.blogspot.com/2023/12/tbd.html#upperBound
        upperBound = vx0 * one_kdt / k;
        // Debug.Log("upper " + upperBound);

        // 最低値の最適な決め方が不明、とりあえず最大値の半分にしておく
        lowerBound = upperBound / 2;
        
        distance = Bisection.FindRoot(F, lowerBound, upperBound, accuracy);
    }

    /// <summary>
    /// f(x)
    /// </summary>
    /// <param name="x"></param>
    /// <returns> TBD </returns>
    public double F(double x)
    {
        return  -reciprocalOfCoefficientOfX + (reciprocalOfCoefficientOfX - x)
        * Math.Pow(one_kdt, coefficient2OfX * x + y0Term);
    }

    public double GetDistance()
    {
        return distance;
    }
}
