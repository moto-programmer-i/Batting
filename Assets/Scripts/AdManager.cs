using System;
using System.Collections.Generic;
using Unity.Services.LevelPlay;
using UnityEngine;

// 広告の設定で必要
// https://developers.is.com/ironsource-mobile/unity/unity-plugin-japanese/#step-5
public class AdManager : MonoBehaviour
{
    [SerializeField]
    private string appKey;

    [SerializeField, TextArea]
    private List<string> messages = new ();

    [SerializeField]
    private SaveDataManager saveDataManager;

    void Awake()
    {
        // // テスト用
        // IronSource.Agent.setMetaData("is_test_suite","enable");

        // APIがなぜかdeprecatedの方のクラスを引数にしてるため従うしかない
        // 謎すぎて対応方法が不明
        // LevelPlayAdFormat[] legacyAdFormats = new[] { LevelPlayAdFormat.REWARDED };
        // LevelPlay.Init(appKey, userId, legacyAdFormats);
        LevelPlay.Init(appKey, null, new[] { com.unity3d.mediation.LevelPlayAdFormat.REWARDED });
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        //  設定が正常に取得されなかったため、広告を読み込むことができません。後で (インターネット接続が利用可能なとき、または失敗の理由が解決されたとき)、ironSource SDK の初期化を再試行することをお勧めします。
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

        // 広告のイベント設定
        // IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
    }

    void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        // // テスト用
        // IronSource.Agent.launchTestSuite();
    }

    void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        // 本当は再試行するべきらしいが、今回は別に良いので今はやめておく
        AndroidUtils.ShowDialog("広告初期化エラー", error.ErrorMessage);
    }


    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    // void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    // {
    //     AndroidUtils.ShowDialog("Closed", "");
    //     // saveDataManager.AddAfterLoad(saveData => {
    //     //     // 順番に開発裏話を表示
    //     //     if (saveData.DevelopmentMessageIndex < 0 || saveData.DevelopmentMessageIndex >= messages.Count) {
    //     //         saveData.DevelopmentMessageIndex = 0;
    //     //     }
    //     //     AndroidUtils.ShowDialog("開発裏話", messages[saveData.DevelopmentMessageIndex]);
    //     //     ++saveData.DevelopmentMessageIndex;
    //     // });
    // }

    // The user completed to watch the video, and should be rewarded.
    // The placement parameter will include the reward data.
    // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {        
        saveDataManager.AddAfterLoad(saveData => {
            // 順番に開発裏話を表示
            if (saveData.DevelopmentMessageIndex < 0 || saveData.DevelopmentMessageIndex >= messages.Count) {
                saveData.DevelopmentMessageIndex = 0;
            }
            AndroidUtils.ShowDialog("開発裏話", messages[saveData.DevelopmentMessageIndex]);
            ++saveData.DevelopmentMessageIndex;
            saveDataManager.Save();
        });
    }

    // The rewarded video ad was failed to show.
    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        AndroidUtils.ShowDialog("広告動画表示エラー", error.getDescription());
    }

    /// <summary>
    /// 広告の動画を表示
    /// </summary>
    public void ShowRewardedVideo()
    {
        IronSource.Agent.showRewardedVideo();
    }
}
