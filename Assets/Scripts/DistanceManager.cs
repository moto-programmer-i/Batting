using System;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;

public class DistanceManager : MonoBehaviour
{
    /// <summary>
    /// 実際のマウンドからホームベースまでの距離
    /// </summary>
    const float MOUND_TO_HOME_PLATE_DISTANCE = 18.44f;

    /// <summary>
    /// 距離計算の精度
    /// </summary>
    const float DISTANCE_ACCURACY = 0.1f;

    /// <summary>
    /// 100メガメートルはさすがにキロメートルとして表示用
    /// </summary>
    const int MEGA = 1000000;

    /// <summary>
    /// km 表示用
    /// </summary>
    const int KILO = 1000;

    /// <summary>
    /// マウンドの位置
    /// </summary>
    [SerializeField]
    private Transform mound;

    /// <summary>
    /// ホームベースの位置
    /// </summary>
    [SerializeField]
    private Transform homePlate;

    /// <summary>
    /// 1座標が何mか
    /// </summary>
    private static float meterInCoordinate;

    /// <summary>
    /// ホームベースから見た前のベクトル（マウンドへの向き）
    /// </summary>
    public static Vector3 forward {get; private set;}

    [SerializeField]
    private Canvas distanceCanvas;

    [SerializeField]
    private TextMeshProUGUI distanceText;

    // [SerializeField]
    // private float testDistance;

    
    void Awake()
    {
        
        // double theta = Math.PI / 3;
        // ProjectionDistance distance = new ProjectionDistance(1, 10, theta, 0.3, -Physics.gravity.y, 0, 0.1);

        // Debug.Log("計算飛距離: " + distance.GetDistance());
        
        // 距離の初期設定
        meterInCoordinate = MOUND_TO_HOME_PLATE_DISTANCE / Vector3.Distance(mound.position, homePlate.position);

        // 前のベクトルの初期設定
        forward = mound.position - homePlate.position;
    }

    // void Start()
    // {
    //     ShowDistance(testDistance);
    // }


    /// <summary>
    /// ボールの飛距離を返す
    /// </summary>
    /// <param name="ball">現在のボールの位置</param>
    /// <returns>飛距離(座標値)</returns>
    public float CalcBallDistance(Vector3 ball)
    {
        if (ball == null) {
            return 0f;
        }

        // ホームベースからの距離
        return Vector3.Distance(ball, homePlate.position);
    }

    /// <summary>
    /// ボールの飛距離を返す
    /// </summary>
    /// <param name="ball">現在のボールの位置</param>
    /// <returns>飛距離(m)</returns>
    public float CalcBallDistanceMeter(Vector3 ball)
    {
        return TranslateToMeter(CalcBallDistance(ball));
    }


    /// <summary>
    /// メートルに変換
    /// </summary>
    /// <param name="distance">Unity上での距離(座標値)</param>
    /// <returns>メートル</returns>
    public float TranslateToMeter(float distance)
    {
        return distance * meterInCoordinate;
    }

    /// <summary>
    /// 物体の飛距離を計算
    /// </summary>
    /// <param name="v0">初速</param>
    /// <param name="theta">角度（ラジアン）</param>
    /// <param name="k">空気抵抗</param>
    /// <returns>飛距離(m)</returns>
    public float CalcDistanceMeter(float v0, float theta, float k, float y0 = 0)
    {
        float distance = CalcDistance(v0, theta, k, y0);
        return TranslateToMeter(distance);
    }

    /// <summary>
    /// 物体の飛距離を計算
    /// </summary>
    /// <param name="v0">初速</param>
    /// <param name="theta">角度（ラジアン）</param>
    /// <param name="k">空気抵抗</param>
    /// <returns>飛距離(座標値)</returns>
    public float CalcDistance(float v0, float theta, float k, float y0 = 0)
    {
        // Debug.Log("角度（ラジアン） " + (theta));
        // Debug.Log("角度（°） " + (Mathf.Rad2Deg * theta));
        
        // 空気抵抗が0の場合の0除算回避
        // 参考 https://nekodamashi-math.blog.ss-blog.jp/2019-08-25
        if (k == 0) {
            // v0^2 sin2θ / g
// y0の考慮がされてない、使う場合は要修正。このプロジェクトでは空気抵抗が0になることはない
            return v0 * v0 * MathF.Sin(2 * theta) / -Physics.gravity.y;
        }


        
        // 空気抵抗がある場合の飛距離を求める
        // 参考 https://moto-programmer-i-unity.blogspot.com/2023/12/tbd.html
        ProjectionDistance distance = new ProjectionDistance(Time.fixedDeltaTime, v0, theta, k, -Physics.gravity.y, y0, DISTANCE_ACCURACY);
        return (float)distance.GetDistance();
    }

    /// <summary>
    /// 物体の飛距離を計算
    /// </summary>
    /// <param name="rigidbody">物体</param>
    /// <returns>飛距離(座標値)</returns>
    public float CalcDistance(Rigidbody rigidbody)
    {
        return CalcDistance(
            rigidbody.velocity.magnitude,
            Angle(rigidbody.velocity),
            rigidbody.drag,
            rigidbody.transform.position.y
            );
    }

    /// <summary>
    /// 物体の飛距離を計算
    /// </summary>
    /// <param name="rigidbody">物体</param>
    /// <returns>飛距離(m)</returns>
    public float CalcDistanceMeter(Rigidbody rigidbody)
    {
        return CalcDistanceMeter(
            rigidbody.velocity.magnitude,
            Angle(rigidbody.velocity),
            rigidbody.drag,
            rigidbody.transform.position.y
            );
    }

    /// <summary>
    /// 対象の速度の角度を返す
    /// </summary>
    /// <param name="target">対象の速度</param>
    /// <returns>角度（ラジアン）</returns>
    public static float Angle(Vector3 velocity) {
        return Angle(forward.normalized, velocity.normalized);
    }

    /// <summary>
    /// 一般の角度計算（なぜかVector3.Angleが度数法で角度を返すため用意）
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>角度（ラジアン）</returns>
    // なぜかVector3.Angleが度数法で角度を返すので、内部を拝借
    // 公式にラジアンで返すメソッドができたら修正
    // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Vector3.cs#L324
    public static float Angle(Vector3 from, Vector3 to)
    {
        // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
        float denominator = (float)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
        if (denominator < Vector3.kEpsilonNormalSqrt)
            return 0F;

        float dot = Mathf.Clamp(Vector3.Dot(from, to) / denominator, -1F, 1F);

        // 元のソースでなぜか度数法にしているので、ラジアンで返す
        // return ((float)Math.Acos(dot)) * Mathf.Rad2Deg;

        // Acosで符号が失われるので、y成分の符号をつける
        return MathF.Sign(to.y) * MathF.Acos(dot);
    }

    public async void ShowDistance(float distance)
    {
        ShowDistanceCanvas(true);
        
        // メガメートルを超えればkm表示
        if (distance >= MEGA) {
            distanceText.text = (distance / KILO).ToString("F0") + "km";
        } else {
            distanceText.text = distance.ToString("F0") + "m";
        }
        

        // // テキスト設定変更
        // try {
        //     DistanceSetting setting = SettingsManager.DistanceSettings.fromDistance(integerDistance);
        //     distanceText.fontSize = setting.Size;
        //     distanceText.color = setting.Color;
        //     distanceText.outlineColor = setting.OutlineColor;
        //     distanceText.fontWeight = setting.FontWeight;

        // // 飛距離が想定外の場合は表示しない
        // } catch (Exception e) {
        //     Debug.LogError(e);
        // }  
        

        // メソッド全体をasyncにしなければ、enabledが反映されなかった
        // // 指定時間後に非表示にする
        // Task.Run(async()  => {
        //     await Task.Delay(distanceDisplayMiliSeconds);
        //     ShowDistanceCanvas(false);
        //     distanceText.enabled = false;
        //     });

        // 指定時間後に非表示にする
        await Task.Delay(SettingsManager.DistanceSettings.DistanceDisplayMiliSeconds);
        ShowDistanceCanvas(false);
    }

    public void ShowDistanceCanvas(bool visible)
    {
        
        // distanceCanvas.enabled = visible; 
        // これだとGameObjectをいじるので、nullエラーが発生する
        // 参考 https://discussions.unity.com/t/how-to-enable-and-disable-a-canvas-window-by-scripting/117242/3

        // 内部のキャンバスだけを非表示
        distanceCanvas.GetComponent<Canvas>().enabled = visible;
    }
}
