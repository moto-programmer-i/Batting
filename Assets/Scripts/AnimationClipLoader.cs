using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class AnimationClipLoader
{

    // https://docs.unity3d.com/ja/2020.3/ScriptReference/AnimationClip.SetCurve.html
    // Transform.localPosition のようにTransformの変数一覧がキーになるらしい
    /// <summary>
    /// 相対x座標をSetCurveする場合のキー
    /// </summary>
    const string LOCAL_POSITION_X_KEY = "localPosition.x";
    /// <summary>
    /// 相対y座標をSetCurveする場合のキー
    /// </summary>
    const string LOCAL_POSITION_Y_KEY = "localPosition.y";
    /// <summary>
    /// 相対z座標をSetCurveする場合のキー
    /// </summary>
    const string LOCAL_POSITION_Z_KEY = "localPosition.z";
    

    /// <summary>
    /// AnimationClipをAnimatorに設定
    /// </summary>
    /// <param name="jsonFilename"></param>
    /// <param name="clipname"></param>
    /// <param name="anim"></param>
    public static void setClip(string jsonFilename, string clipname, Animator anim)
    {
        // jsonからAnimationClipを作成
        AnimationCurveJson curve = ResourceUtils.LoadJson<AnimationCurveJson>(jsonFilename);
        AnimationClip clip = convertToAnimationClip(curve);
        clip.name = clipname;

        // AnimationClipを変更（AnimatorOverrideControllerを経由しないと動かない）
		AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animatorOverrideController;
        animatorOverrideController[clip.name] = clip;
    }

    /// <summary>
    /// JsonからAnimationClipに変換
    /// </summary>
    /// <param name="curve"></param>
    /// <returns></returns>
    public static AnimationClip convertToAnimationClip(AnimationCurveJson curve)
    {
        if (curve == null || ListUtils.IsEmpty(curve.keyframes)) {
            return null;
        }

        AnimationClip clip = new AnimationClip();

        // Positionの成分ごとのカーブを作成
        // 参考
        // https://im0039kp.jp/%E3%80%90unity%E3%80%91animationclip%E3%82%92%E3%82%B9%E3%82%AF%E3%83%AA%E3%83%97%E3%83%88%E3%81%8B%E3%82%89%E7%94%9F%E6%88%90/
        AnimationCurve xCurve = new AnimationCurve();
        AnimationCurve yCurve = new AnimationCurve();
        AnimationCurve zCurve = new AnimationCurve();
        for(int i = 0; i < curve.keyframes.Count; ++i) {
            xCurve.AddKey(curve.keyframes[i].time, curve.keyframes[i].position.x);
            yCurve.AddKey(curve.keyframes[i].time, curve.keyframes[i].position.y);
            zCurve.AddKey(curve.keyframes[i].time, curve.keyframes[i].position.z);
        }
        clip.SetCurve("", typeof(Transform), LOCAL_POSITION_X_KEY, xCurve);
        clip.SetCurve("", typeof(Transform), LOCAL_POSITION_Y_KEY, yCurve);
        clip.SetCurve("", typeof(Transform), LOCAL_POSITION_Z_KEY, zCurve);


        return clip;
    }
}
