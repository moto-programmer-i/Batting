using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DistanceTester : MonoBehaviour
{
    [SerializeField]
    private GameObject ballPrefab;

    /// <summary>
    /// ボールのスピード
    /// </summary>
    [SerializeField]
    private float v0;

    
    /// <summary>
    /// PIの分子（ラジアンで角度を指定したいため）
    /// </summary>
    [SerializeField]
    private float numeratorOfPi;

    /// <summary>
    /// PIの分母（ラジアンで角度を指定したいため）
    /// </summary>
    [SerializeField]
    private float denominatorOfPi;

    private float theta;

    [SerializeField]
    private Transform releasePoint;

    private Rigidbody sample;
    private float time = 0;

    private int count = 0;

    [SerializeField]
    private int maxCount;

    [SerializeField]
    private float drag;

    [SerializeField]
    private float mass;

    private List<String> lines = new ();

    [SerializeField]
    private DistanceManager distanceManager;

    private Vector3Data velocity0;

    private float k;
    private float g;
    private float dt;

    private float one_kdt;

    void Start()
    {
        k = drag;
        g = -Math.Abs(Physics.gravity.y);
        dt = Time.fixedDeltaTime;
        one_kdt = 1 - k * dt;
        ThrowBall();
        
    }

    void FixedUpdate()
    {
        if (sample == null || sample.transform.position.y < 0) {
            return;
        }
        // if (lines.Count >= maxCount) {
        //     lines.RemoveAt(0);
        // }
        ++count;
        // Debug.Log(
        //     "t," + time
        // + "\nx," + r.transform.position.x
        // + "\ny," + r.transform.position.y
        // );
        time += Time.deltaTime;
        // lines.Add(time + "," + Mathf.Abs(sample.velocity.x) + "," + Mathf.Abs(sample.position.x)
        //  );

        //  lines.Add(time + "," + sample.velocity.y + "," + sample.position.y
        //  );
        // lines.Add($"y: {sample.position.y}");

        // 速度xの一般項
        // lines.Add($"vx: {sample.velocity.x}");
        // float vx = velocity0.X  * MathF.Pow(one_kdt, count);
        // lines.Add($"\ncalc vx: {vx}");

        // 位置xの一般項
        // lines.Add($"x: {sample.position.x}");
        // float x = velocity0.X * one_kdt * (1 - MathF.Pow(one_kdt, count)) / k;
        // lines.Add($"\ncalc x: {x}");
        
        
        
        
        // 速度yの一般項
        // lines.Add($"vy: {sample.velocity.y}");
        // float vy = (velocity0.Y - g * (1 - k * dt)/k) * MathF.Pow(1 - k * dt, count) + g * (1 - k * dt) / k;
        // lines.Add($"\ncalc vy: {vy}");

        // 位置yの一般項
        // lines.Add($"y: {sample.position.y}");
        // 
        // float y = (velocity0.Y - g * one_kdt/k) * one_kdt * (1- MathF.Pow(one_kdt, count))/ k + count * g * dt * one_kdt / k;
        // lines.Add($"\ncalc y: {releasePoint.position.y + y}");
    }

    public void Init()
    {
        theta = Mathf.PI * numeratorOfPi / denominatorOfPi;
    }

    public void ThrowBall() {
        Init();
        GameObject ball = Instantiate(ballPrefab, releasePoint.position, Quaternion.identity);
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();

        // テスト用の値に
        ballRigidbody.drag = this.drag;
        ballRigidbody.mass = this.mass;

        sample = ballRigidbody;
        count = 0;
        // Debug.Log("time,vx,x");
        // Debug.Log("time,vy,y");

        ballRigidbody.velocity = Quaternion.Euler(0, 0, -theta * Mathf.Rad2Deg) * Vector3.left * v0;
        velocity0 = Vector3Data.From(ballRigidbody.velocity);
        float distance = BattingInstances.GetDistanceManager().CalcDistance(ballRigidbody);
        Debug.Log("計算飛距離（座標値） " +  distance);

        // 飛距離表示
        distanceManager.ShowDistance(distance);
        
        // 一旦地面に着いたときの距離
        BallEventTrigger trigger = ball.GetComponent<BallEventTrigger>();
        trigger.OnBounce(ball => {
            // Debug.Log("飛距離（m）: " + BattingInstances.GetDistanceManager().CalcBallDistance(ball));
            float realDistance = BattingInstances.GetDistanceManager().CalcBallDistance(ball);
            Debug.Log("飛距離（座標値）: " + realDistance);
            Debug.Log($"x:{ball.x}, y:{ball.y}, z:{ball.z}");

            // もう1度投げる
            // ThrowBall();

            // Debug.Log("t," + time);
            Debug.Log(string.Join("\n", lines));
        });

        time = 0;
    }
}
