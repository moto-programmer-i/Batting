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
    // https://docs.unity3d.com/jp/540/ScriptReference/JsonUtility.FromJson.html
    // public float time { get; set; }
    // public Vector3 position { get; set; }

    public float time;
    public Vector3Data position;
    public Vector3Data rotation;

    // プログラム上はenumにすべきだが、数値でシリアライズされるのが微妙？要検討
    public SwingType? type;

    public AnimationKeyframe(){}

    public AnimationKeyframe(float time, Vector3Data position, Vector3Data rotation, SwingType? type)
    {
        this.time = time;
        this.position = position;
        this.rotation = rotation;
        this.type = type;
    }
}
