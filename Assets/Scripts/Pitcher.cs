using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;

public class Pitcher : MonoBehaviour
{
    [SerializeField]
    private GameObject ballPrefab;

    /// <summary>
    /// ボールのスピード
    /// </summary>
    [SerializeField]
    private Vector3 force = new Vector3(1,0,0);

    // [SerializeField]
    // private float idleSecond = 3;

    [SerializeField]
    private Transform releasePoint;

    // Start is called before the first frame update
    void Start()
    {
        // throwBall();
    }

    // Update is called once per frame
    void Update()
    {
        // // idleSecondごとにボールを投げる
        // elapsedTime += Time.deltaTime;
        // if (elapsedTime >= idleSecond) {
        //     elapsedTime = 0;
        //     ThrowBall();
        // }
    }

    public void ThrowBall() {
        GameObject ball = Instantiate(ballPrefab, releasePoint.position, Quaternion.identity);
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        ballRigidbody.AddForce(force);

        // 一旦地面に着いたときの距離
        BallEventTrigger trigger = ball.GetComponent<BallEventTrigger>();
        trigger.OnBounce(ball => {
            Debug.Log("飛距離: " + BattingInstances.GetDistanceManager().CalcBallDistance(ball));
        });
    }
}
