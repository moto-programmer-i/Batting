using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationCurveJson
{
    // これが正規の書き方のはずだが、Jsonから値を設定できない
    // public List<AnimationKeyframe> keyframes { get; set; }
    public List<AnimationKeyframe> keyframes = new ();
}
