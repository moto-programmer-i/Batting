using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveJson
{
    
    private List<AnimationKeyframe> _keyframes = new();
    public List<AnimationKeyframe> Keyframes {
        get {
            return _keyframes;
        }
    
        set
        {
            _keyframes = value;
            initImpactIndex();
        }
    }

    /// <summary>
    /// インパクトの添え字
    /// </summary>
    public int ImpactIndex {get; private set;}

    /// <summary>
    /// インパクトの添え字を初期化。リスト変更後などに呼び出すこと。
    /// </summary>
    public void initImpactIndex()
    {
        for(int i = 0; i < _keyframes.Count; ++i) {
            if (SwingType.IMPACT == _keyframes[i].Type) {
                ImpactIndex = i;
                return;
            }
        }

    }

    // 参考
    // https://stackoverflow.com/a/49415723
    [OnDeserialized()]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        initImpactIndex();
    }
}
