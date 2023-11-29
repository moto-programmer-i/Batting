using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AnimationClipの1キーフレームの情報
/// </summary>
public class AnimationKeyframe
{
    // これが正規の書き方のはずだが、JsonUtilityは値を設定できない
    // https://docs.unity3d.com/jp/540/ScriptReference/JsonUtility.FromJson.html
    // public float time { get; set; }
    // public Vector3 position { get; set; }

    public float Time { get; set; }
    public Vector3Data Position { get; set; }
    public Vector3Data Rotation { get; set; }

    // プログラム上はenumにすべきだが、数値でシリアライズされるのが微妙？要検討
    public SwingType? Type { get; set; }

    public AnimationKeyframe(){}

    public AnimationKeyframe(float time, Vector3Data position = null, Vector3Data rotation = null, SwingType? type = null)
    {
        this.Time = time;
        this.Position = position;
        this.Rotation = rotation;
        this.Type = type;
    }

    public AnimationKeyframe Clone()
    {
        return new AnimationKeyframe(
            this.Time,
            this.Position?.Clone(),
            this.Rotation?.Clone(),
            this.Type
            );
    }
}
