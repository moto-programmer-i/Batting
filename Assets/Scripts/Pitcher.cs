using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitcher : MonoBehaviour
{
    public GameObject ballPrefab;

    /// <summary>
    /// ボールのスピード
    /// </summary>
    public Vector3 force = new Vector3(1,0,0);

    public float idleSecond = 3;

    private float elapsedTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        // throwBall();
    }

    // Update is called once per frame
    void Update()
    {
        // idleSecondごとにボールを投げる
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= idleSecond) {
            elapsedTime = 0;
            throwBall();
        }
    }

    void throwBall() {
        GameObject ball = Instantiate(ballPrefab, this.transform.position, Quaternion.identity);
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        ballRigidbody.AddForce(force);
    }
}
