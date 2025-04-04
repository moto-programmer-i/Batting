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
    /// 回転x成分をSetCurveする場合のキー
    /// </summary>
    const string ROTATION_X_KEY = "localRotation.x";
    /// <summary>
    /// 回転y成分をSetCurveする場合のキー
    /// </summary>
    const string ROTATION_Y_KEY = "localRotation.y";
    /// <summary>
    /// 回転z成分をSetCurveする場合のキー
    /// </summary>
    const string ROTATION_Z_KEY = "localRotation.z";
    /// <summary>
    /// 回転w成分をSetCurveする場合のキー
    /// </summary>
    const string ROTATION_W_KEY = "localRotation.w";
    

    // WebGLだと動作しなかった
    /// <summary>
    /// AnimationClipをAnimatorに設定
    /// </summary>
    /// <param name="jsonFilename"></param>
    /// <param name="clipname"></param>
    /// <param name="anim"></param>
    // public static void setClip(AnimationCurveJson curve, string clipname, Animator anim)
    // {
    //     // AnimationCurveJsonからAnimationClipを作成
    //     AnimationClip clip = convertToAnimationClip(curve);
    //     clip.name = clipname;

    //     // AnimationClipを変更（AnimatorOverrideControllerを経由しないと動かない）
    //     if (anim.runtimeAnimatorController is AnimatorOverrideController) {
    //         ((AnimatorOverrideController)anim.runtimeAnimatorController)[clip.name] = clip;
    //     } else {
    //         AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
    //         animatorOverrideController[clip.name] = clip;
    //         anim.runtimeAnimatorController = animatorOverrideController;
    //     }
    // }

    public static void setClip(AnimationCurveJson curve, string clipname, Animation anim)
    {
        // AnimationCurveJsonからAnimationClipを作成
        AnimationClip clip = convertToAnimationClip(curve);
        clip.name = clipname;
        anim.AddClip(clip, clipname);
    }

    /// <summary>
    /// JsonからAnimationClipに変換
    /// </summary>
    /// <param name="curve"></param>
    /// <returns></returns>
    public static AnimationClip convertToAnimationClip(AnimationCurveJson curve)
    {
        if (curve == null || ListUtils.IsEmpty(curve.Keyframes)) {
            return null;
        }

        AnimationClip clip = new AnimationClip();
        
        // WebGLビルド エラー対処
        // Can't use AnimationClip::SetCurve at Runtime on non Legacy AnimationClips
        clip.legacy = true;

        // Positionの成分ごとのカーブを作成
        // 参考
        // https://im0039kp.jp/%E3%80%90unity%E3%80%91animationclip%E3%82%92%E3%82%B9%E3%82%AF%E3%83%AA%E3%83%97%E3%83%88%E3%81%8B%E3%82%89%E7%94%9F%E6%88%90/
        AnimationCurve xPositionCurve = new AnimationCurve();
        AnimationCurve yPositionCurve = new AnimationCurve();
        AnimationCurve zPositionCurve = new AnimationCurve();
        AnimationCurve xRotationCurve = new AnimationCurve();
        AnimationCurve yRotationCurve = new AnimationCurve();
        AnimationCurve zRotationCurve = new AnimationCurve();
        AnimationCurve wRotationCurve = new AnimationCurve();
        for(int i = 0; i < curve.Keyframes.Count; ++i) {
            if (curve.Keyframes[i].Position != null) {
                xPositionCurve.AddKey(curve.Keyframes[i].Time, curve.Keyframes[i].Position.X);
                yPositionCurve.AddKey(curve.Keyframes[i].Time, curve.Keyframes[i].Position.Y);
                zPositionCurve.AddKey(curve.Keyframes[i].Time, curve.Keyframes[i].Position.Z);
            }
            
            if (curve.Keyframes[i].Rotation != null) {
                // rotationは一度変換が必要
                // 参考 https://monaski.hatenablog.com/entry/2015/11/15/172907
                Quaternion angle = Quaternion.Euler(curve.Keyframes[i].Rotation.X, curve.Keyframes[i].Rotation.Y, curve.Keyframes[i].Rotation.Z);
                
                xRotationCurve.AddKey(curve.Keyframes[i].Time, angle.x);
                yRotationCurve.AddKey(curve.Keyframes[i].Time, angle.y);
                zRotationCurve.AddKey(curve.Keyframes[i].Time, angle.z);
                wRotationCurve.AddKey(curve.Keyframes[i].Time, angle.w);
            }
        }
        clip.SetCurve("", typeof(Transform), LOCAL_POSITION_X_KEY, xPositionCurve);
        clip.SetCurve("", typeof(Transform), LOCAL_POSITION_Y_KEY, yPositionCurve);
        clip.SetCurve("", typeof(Transform), LOCAL_POSITION_Z_KEY, zPositionCurve);

        clip.SetCurve("", typeof(Transform), ROTATION_X_KEY, xRotationCurve);
        clip.SetCurve("", typeof(Transform), ROTATION_Y_KEY, yRotationCurve);
        clip.SetCurve("", typeof(Transform), ROTATION_Z_KEY, zRotationCurve);
        clip.SetCurve("", typeof(Transform), ROTATION_W_KEY, wRotationCurve);

        // legacyがtrueのままだとその先で動かないのでfalseに戻す
        // 明らかに正当な手段ではないが、正しい方法不明
        // The legacy Animation Clip "Swing" cannot be used in the Override Controller "". Legacy Animation Clips are not allowed in Animator Override Controllers.
        // clip.legacy = false;

        return clip;
    }
}