using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
            Init();
        }
    }

    /// <summary>
    /// インパクトの添え字
    /// </summary>
    public int ImpactIndex {get; private set;}

    /// <summary>
    /// インパクトまでの時間
    /// </summary>
    public float TimeToImpact {get; private set;}

    /// <summary>
    /// インパクトの添え字などを初期化。リスト変更後などに呼び出すこと。
    /// </summary>
    public void Init()
    {
        for(int i = 0; i < _keyframes.Count; ++i) {
            if (SwingType.IMPACT == _keyframes[i].Type) {
                ImpactIndex = i;
                break;
            }
        }

        // インパクトまでのスイングの時間を初期化
        TimeToImpact =  _keyframes[ImpactIndex].Time - _keyframes.First().Time;

    }

    /// <summary>
    /// 。
    /// 事前にInitImpactIndex()を呼んでおくこと
    /// </summary>
    public void InitSwingXWidthToImpact()
    {
        
    }

    // 参考
    // https://stackoverflow.com/a/49415723
    [OnDeserialized()]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        Init();
    }
}
