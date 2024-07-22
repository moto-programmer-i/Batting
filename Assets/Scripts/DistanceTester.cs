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
    private int maxCount = 2;

    [SerializeField]
    private float drag;

    [SerializeField]
    private float mass;

    private List<String> lines = new ();

    // Start is called before the first frame update
    void Start()
    {
        ThrowBall();
        
    }

    void FixedUpdate()
    {
        
        // if (sample == null || sample.transform.position.y < 0) {
        //     return;
        // }
        if (count >= maxCount) {
            if (lines.Any()) {
                // Debug.Log(String.Join("\n", lines));
                lines.Clear();
            }
            return;
        }
        ++count;
        // Debug.Log(
        //     "t," + time
        // + "\nx," + r.transform.position.x
        // + "\ny," + r.transform.position.y
        // );
        time += Time.deltaTime;
        // lines.Add(time + "," + Mathf.Abs(sample.velocity.x) + "," + Mathf.Abs(sample.position.x)
        //  );

         lines.Add(time + "," + sample.velocity.y + "," + sample.position.y
         );
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
        float distance = BattingInstances.GetDistanceManager().CalcDistance(ballRigidbody);
        Debug.Log("計算飛距離（座標値） " +  distance);
            
        
        // 一旦地面に着いたときの距離
        BallEventTrigger trigger = ball.GetComponent<BallEventTrigger>();
        trigger.OnBounce(ball => {
            // Debug.Log("飛距離（m）: " + BattingInstances.GetDistanceManager().CalcBallDistance(ball));
            float realDistance = BattingInstances.GetDistanceManager().CalcBallDistance(ball);
            Debug.Log("飛距離（座標値）: " + realDistance);

            // もう1度投げる
            // ThrowBall();

            // Debug.Log("t," + time);
        });

        time = 0;
    }
}
