using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AnimationClipの1キーフレームの情報
/// </summary>
[System.Serializable]
public class AnimationKeyframe
{
    // これが正規の書き方のはずだが、Jsonから値を設定できない
    // public float time { get; set; }
    // public Vector3 position { get; set; }

    public float time;
    public Vector3 position;
    public Vector3 rotation;

    public AnimationKeyframe(){}

    public AnimationKeyframe(float time, Vector3 position, Vector3 rotation)
    {
        this.time = time;
        this.position = position;
        this.rotation = rotation;
    }
}
