using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationCurveJson
{
    // これが正規の書き方のはずだが、Jsonから値を設定できない
    // public List<AnimationKeyframe> keyframes { get; set; }
    public List<AnimationKeyframe> keyframes = new ();

    /// <summary>
    /// インパクトの添え字
    /// </summary>
    public int ImpactIndex {get; set;}

    /// <summary>
    /// インパクトの添え字を初期化。JSONから生成後などに呼び出すこと。
    /// </summary>
    public void initImpactIndex()
    {
        for(int i = 0; i < keyframes.Count; ++i) {
            if (SwingType.IMPACT == keyframes[i].type) {
                ImpactIndex = i;
                return;
            }
        }

    }
}
