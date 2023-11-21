using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;
using System;

public class SwingConverter
{
    public const string TIME_KEY =  "time";
    public const string POSITION_CURVE_KEY = "m_PositionCurves";
    public const string SCALE_CURVE_KEY = "m_ScaleCurves";
    public const string ANIMATION_FILENAME = "SampleSwing.anim";
    public const string ANIMATION_FILEPATH = "Assets/Resources/Animations";
    public const string JSON_FILENAME = "BaseSwing.json";
    

    private static readonly Regex DECIMAL_PATTERN = new Regex("[-+]?\\d+\\.?\\d*");

    /// <summary>
    /// スイングの変換（テストではないが、この方が実装が楽だった。他の実装方法があれば要改善）
    /// </summary>
    [Test]
    public void ConvertSwing()
    {
        // ファイル読み込み
        AnimationCurveJson json = new ();
        FileUtils.ReadFile(handler => {
                // 先に回転の情報（m_EulerCurves）が.animに書かれているので読み込み
                while(ReadRotation(POSITION_CURVE_KEY, handler, json));

                // 位置の情報を読み込み
                while(ReadPosition(SCALE_CURVE_KEY, handler, json));
            },

            // 変換元ファイル
            ANIMATION_FILENAME, ANIMATION_FILEPATH
        );

        // 初期化しておく
        json.Init();

        // JSON書き出し
        FileUtils.SaveJson(json, JSON_FILENAME, ANIMATION_FILEPATH);
    }

    bool ReadKeyframe(string until, StreamReader handler, AnimationCurveJson json, Action<AnimationKeyframe, MatchCollection> valueHandler)
    {
        // 最初のデータまで読み飛ばす
        string line;
        do {
            line = handler.ReadLine();

            // 末尾または終了文字列に到達した場合はfalse
            if (line == null || line.Contains(until)) {
                return false;
            }
        }
        while(!line.Contains(TIME_KEY));

        // time（time: 0）などを読み込み
        float time = float.Parse(DECIMAL_PATTERN.Match(line).Value);

        // 該当のtimeがあれば更新
        AnimationKeyframe keyframe = json.Keyframes.Find(e => e.Time == time);
        if (keyframe == null) {
            keyframe = new (time);
            json.Keyframes.Add(keyframe);
        }

        // value: {x: 90, y: 158.6, z: 0}などを読み込み
        MatchCollection values = DECIMAL_PATTERN.Matches(handler.ReadLine());
        valueHandler.Invoke(keyframe, values);

        // 正常終了
        return true;
    }

    bool ReadRotation(string until, StreamReader handler, AnimationCurveJson json)
    {
        return ReadKeyframe(until, handler, json, (keyframe, rotation) => {
            keyframe.Rotation = new Vector3Data(
            float.Parse(rotation[Vector3Data.X_INDEX].Value),
            float.Parse(rotation[Vector3Data.Y_INDEX].Value),
            float.Parse(rotation[Vector3Data.Z_INDEX].Value));

            // 回転のyとzが両方0ならばインパクト位置
            if (keyframe.Rotation.Y == 0 && keyframe.Rotation.Z == 0) {
                keyframe.Type = SwingType.IMPACT;
            }
        });
    }

    bool ReadPosition(string until, StreamReader handler, AnimationCurveJson json)
    {
        return ReadKeyframe(until, handler, json, (keyframe, position) => {
            keyframe.Position = new Vector3Data(
            float.Parse(position[Vector3Data.X_INDEX].Value),
            float.Parse(position[Vector3Data.Y_INDEX].Value),
            float.Parse(position[Vector3Data.Z_INDEX].Value));
        });
    }
}
