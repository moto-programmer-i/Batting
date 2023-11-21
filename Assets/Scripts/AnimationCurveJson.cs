using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography;

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
        Sort();

        // ImpactIndexの初期化
        for(int i = 0; i < _keyframes.Count; ++i) {
            if (SwingType.IMPACT == _keyframes[i].Type) {
                ImpactIndex = i;
                break;
            }
        }

        // インパクトまでのスイングの時間を初期化
        TimeToImpact =  _keyframes[ImpactIndex].Time - _keyframes.First().Time;
    }

    public void Sort()
    {
        _keyframes.Sort((a, b) => {
                // Math.Abs(a.Time - b.Time)の戻り値がなぜかintじゃない
                float sign = a.Time - b.Time;
                if (sign == 0) {
                    return 0;
                }
                if (sign < 0) {
                    return -1;
                }
                return 1;
            }

            );
    }

    // 参考
    // https://stackoverflow.com/a/49415723
    [OnDeserialized()]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        Init();
    }
}
