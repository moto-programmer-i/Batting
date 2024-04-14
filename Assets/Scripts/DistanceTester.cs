using System;
using System.Collections;
using System.Collections.Generic;
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
    /// PIの分母（ラジアンで角度を指定したいため）
    /// </summary>
    [SerializeField]
    private float denominatorOfPi;

    private float theta;

    [SerializeField]
    private Transform releasePoint;

    // Start is called before the first frame update
    void Start()
    {
        ThrowBall();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        theta = Mathf.PI / denominatorOfPi;
    }

    public void ThrowBall() {
        Init();
        GameObject ball = Instantiate(ballPrefab, releasePoint.position, Quaternion.identity);
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();

        ballRigidbody.velocity = Quaternion.Euler(0, 0, -theta * Mathf.Rad2Deg) * Vector3.left * v0;

        BattingInstances.GetDistanceManager().CalcDistance(
            ballRigidbody.mass,
            v0,
            theta,
            ballRigidbody.drag,
            ballRigidbody.transform.position.y);
            
        
        // 一旦地面に着いたときの距離
        BallEventTrigger trigger = ball.GetComponent<BallEventTrigger>();
        trigger.OnBounce(ball => {
            // Debug.Log("飛距離（m）: " + BattingInstances.GetDistanceManager().CalcBallDistance(ball));
            BattingInstances.GetDistanceManager().CalcBallDistance(ball);

            // もう1度投げる
            // ThrowBall();
        });
    }
}
