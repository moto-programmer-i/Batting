using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEventTrigger : MonoBehaviour
{
    /// <summary>
    /// 飛んでいるボールのレイヤー
    /// </summary>
    public const int FLYING_BALL_LAYER = 8;

    [SerializeField]
    private Transform ball;
    
    private List<Action<Vector3>> onBounceListeners = new ();

    public bool Hit {get; set;}

    public void OnBounce(Action<Vector3> listener)
    {
        onBounceListeners.Add(listener);
    }

    void FixedUpdate()
    {
        // グラウンドのサイズが限られているので、yが0以下のときも地面についたとする
        if(ball.transform.position.y <= 0){
            Invoke();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // グラウンドと接触で地面についたものとする
        if (GameObjectTags.GROUND.Equals(collision.gameObject.tag)) {
            Invoke();
        }
    }

    /// <summary>
    /// 地面についた時の処理
    /// </summary>
    private void Invoke()
    {
            onBounceListeners.ForEach(listener => listener.Invoke(ball.position));

            // とりあえず1回きりで
            onBounceListeners.Clear();
    }
}
