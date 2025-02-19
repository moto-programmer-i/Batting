using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography;
using System.Numerics;

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
        _keyframes.Sort((a, b) => Math.Sign(a.Time - b.Time));
    }

    // デシリアライズ時にメソッド呼び出し
    // 参考
    // https://stackoverflow.com/a/49415723
    [OnDeserialized()]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        Init();
    }

    /// <summary>
    /// toが起点になるようにずらす
    /// </summary>
    /// <param name="to">新しい起点</param>
    public void Offset(Transform to)
    {
       
        
        
        AnimationKeyframe first = _keyframes.First();

        // toが起点になるように差を作成
        Vector3Data offset = Vector3Data.From(to.position) - first.Position;

        Debug.Log("from: " + first.Position);
        Debug.Log("to: " + to.position);
        Debug.Log("offset: " + offset);

        // toを起点に設定
        first.Position = Vector3Data.From(to.position);

        // 全ての要素に差を適用
        for (int i = 1; i < _keyframes.Count; ++i) {
            _keyframes[i].Position += offset;
            Debug.Log($"{i}: {_keyframes[i].Position}");
        }
    }
}
