using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceManager : MonoBehaviour
{
    /// <summary>
    /// 実際のマウンドからホームベースまでの距離
    /// </summary>
    const float MOUND_TO_HOME_PLATE_DISTANCE = 18.44f;

    /// <summary>
    /// マウンドの位置
    /// </summary>
    [SerializeField]
    private Transform mound;

    /// <summary>
    /// ホームベースの位置
    /// （要修正：現状バットのインスタンスを置かないと距離がマイナスになってしまったりする）
    /// </summary>
    [SerializeField]
    private Transform homePlate;

    /// <summary>
    /// 座標上での距離（この値を18.44mとして換算）
    /// </summary>
    private static float distanceInCoordinate;

    /// <summary>
    /// ホームベースから見た前のベクトル（マウンドへの向き）
    /// </summary>
    public static Vector3 forward {get; private set;}

    
    void Start()
    {        
        // 距離の初期設定
        distanceInCoordinate = Vector3.Distance(mound.position, homePlate.position);

        // 前のベクトルの初期設定
        forward = mound.position - homePlate.position;

        Debug.Log("前のベクトル: " + forward.normalized);
    }

    
    void Update()
    {
        
    }

    /// <summary>
    /// ボールの飛距離を返す
    /// </summary>
    /// <param name="ball">現在のボールの位置</param>
    /// <returns>飛距離(m)</returns>
    public float CalcBallDistance(Vector3 ball)
    {
        if (ball == null) {
            return 0f;
        }

        return TranslateToMeter(Vector3.Distance(ball, homePlate.position));
    }

    /// <summary>
    /// メートルに変換
    /// </summary>
    /// <param name="distance">Unity上での距離</param>
    /// <returns>メートル</returns>
    public float TranslateToMeter(float distance)
    {
        return MOUND_TO_HOME_PLATE_DISTANCE * distance / distanceInCoordinate;
    }

    /// <summary>
    /// 物体の飛距離を計算
    /// </summary>
    /// <param name="m">質量</param>
    /// <param name="v0">初速</param>
    /// <param name="theta">角度（ラジアン）</param>
    /// <param name="k">空気抵抗</param>
    /// <returns>飛距離(m)</returns>
    public float CalcDistance(float m, float v0, float theta, float k)
    {
        // 空気抵抗が0の場合の0除算回避
        // 参考 https://nekodamashi-math.blog.ss-blog.jp/2019-08-25
        if (k == 0) {
            // v0^2 sin2θ / g
            return v0 * v0 * MathF.Sin(2 * theta) / Physics.gravity.y;
        }
        
        // 空気抵抗がある場合の飛距離は妥協して m * v0 * cosθ / k で求める
        // 参考 https://moto-programmer-i-unity.blogspot.com/2023/12/tbd.html
        return TranslateToMeter(m * v0 * MathF.Cos(theta) / k);
    }

    /// <summary>
    /// 物体の飛距離を計算
    /// </summary>
    /// <param name="rigidbody">物体</param>
    /// <returns>飛距離(m)</returns>
    public float CalcDistance(Rigidbody rigidbody)
    {
        return CalcDistance(
            rigidbody.mass,
            rigidbody.velocity.magnitude,
            Angle(rigidbody.velocity),
            rigidbody.drag
            );
    }

    /// <summary>
    /// 対象の速度の角度を返す
    /// </summary>
    /// <param name="target">対象の速度</param>
    /// <returns></returns>
    public static float Angle(Vector3 velocity) {
        return Vector3.Angle(forward, velocity);
    }
}
