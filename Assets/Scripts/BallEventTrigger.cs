using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEventTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform ball;
    
    private List<Action<Vector3>> onBounceListeners = new ();

    public bool Hit {get; set;}

    public void OnBounce(Action<Vector3> listener)
    {
        onBounceListeners.Add(listener);
    }

    void OnCollisionEnter(Collision collision)
    {
        // グラウンドと接触で地面についたものとする
        if (GameObjectTags.GROUND.Equals(collision.gameObject.tag)) {
            onBounceListeners.ForEach(listener => listener.Invoke(ball.position));

            // とりあえず1回きりで
            onBounceListeners.Clear();
        }
    }
}
