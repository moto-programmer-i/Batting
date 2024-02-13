using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEventTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform ball;
    
    private List<Action<Vector3>> onBounceListeners = new ();

    void Start()
    {
        
    }

    
    void Update()
    {
        // 一旦適当にY <= 0 なら地面に着いたものとする
        if (ball.position.y <= 0) {
            onBounceListeners.ForEach(listener => listener.Invoke(ball.position));

            // とりあえず1回きりで
            onBounceListeners.Clear();
        }
    }

    public void OnBounce(Action<Vector3> listener)
    {
        onBounceListeners.Add(listener);
    }
}
