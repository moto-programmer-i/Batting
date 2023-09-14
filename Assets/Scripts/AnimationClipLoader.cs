using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimationClipLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // // AnimationClip animationClip =  Resources.Load<AnimationClip>("D:\\Unity\\Projects\\Batting\\ExternalSwing.anim");
        // AnimationClip clip =  Resources.Load<AnimationClip>("Animations/TemplateSwing");
        // // EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
        // foreach (var binding in AnimationUtility.GetCurveBindings(clip))
        //     {
        //         AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
        //         // EditorGUILayout.LabelField(binding.path + "/" + binding.propertyName + ", Keys: " + curve.keys.Length);
        //         Debug.Log(curve.ToString());
        //     }
        // // Debug.Log(animationClip.);

        AnimationCurveJson curve = ResourceUtils.LoadJson<AnimationCurveJson>("Animations/TemplateSwing");
        Debug.Log(curve);
        // AnimationKeyframe keyframe = ResourceUtils.LoadJson<AnimationKeyframe>("Animations/temp");
        // Debug.Log(keyframe);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
