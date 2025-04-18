using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 参考
// https://indie-du.com/entry/2016/07/14/200000
// https://docs.unity3d.com/ja/2022.3/Manual/AndroidJavaSourcePlugins.html

public static class AndroidUtils
{
    const string DialogClassName = "batting_center.utils.dialog.DialogUtils";
    const string UnityPlayerClassName = "com.unity3d.player.UnityPlayer";
    const string GetContextMethodName = "currentActivity";

    const string ShowDialogMethodName = "showDialog";

    private static AndroidJavaClass dialogUtils;
    private static AndroidJavaObject androidContext;

    public static void ShowDialog(string title, string message)
    {
#if UNITY_EDITOR
    if (Application.platform == RuntimePlatform.WindowsEditor) {
            Debug.Log($"Androidダイアログ({title})\n{message}");
            return;
    }
#endif

#if UNITY_ANDROID
        if (Application.platform != RuntimePlatform.Android) {
            return;
        }
        // Javaのオブジェクトを作成
        if (dialogUtils == null)
        {
            dialogUtils = new AndroidJavaClass(DialogClassName);
        }


        // Context(Activity)オブジェクトを取得する
        if (androidContext == null)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(UnityPlayerClassName))
            {
                androidContext = unityPlayer.GetStatic<AndroidJavaObject>(GetContextMethodName);
            }
        }

        // showDialog(context, title, message);
        dialogUtils.CallStatic(ShowDialogMethodName, androidContext, title, message);
#endif
    }
}
