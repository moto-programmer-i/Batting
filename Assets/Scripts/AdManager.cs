using Unity.Services.LevelPlay;
using UnityEngine;

// 広告の設定で必要
// https://developers.is.com/ironsource-mobile/unity/unity-plugin-japanese/#step-5
public class AdManager : MonoBehaviour
{
    [SerializeField]
    private string appKey = "";
    [SerializeField]
    private string userId = "";

    // Start is called before the first frame update
    void Awake()
    {
        IronSource.Agent.setMetaData("is_test_suite", "enable");
        
        
        
        // Init the SDK when implementing the Multiple Ad Units API for Interstitial and Banner formats, with Rewarded using legacy APIs 
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;

        //  設定が正常に取得されなかったため、広告を読み込むことができません。後で (インターネット接続が利用可能なとき、または失敗の理由が解決されたとき)、ironSource SDK の初期化を再試行することをお勧めします。
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

        // APIがなぜかdeprecatedの方のクラスを引数にしてるため従うしかない
        // 謎すぎて対応方法が不明
        // LevelPlayAdFormat[] legacyAdFormats = new[] { LevelPlayAdFormat.REWARDED };
        // LevelPlay.Init(appKey, userId, legacyAdFormats);
        LevelPlay.Init(appKey, userId, new[] { com.unity3d.mediation.LevelPlayAdFormat.REWARDED });
    }

    void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        //Launch test suite
        IronSource.Agent.launchTestSuite();
        
        // 初期化後、広告のイベント設定
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;

        // 状態を検証（確認後、要削除）
        IronSource.Agent.validateIntegration();
    }

    void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        // 本当は再試行するべきらしいが、方法が不明のため今はやめておく
        Debug.LogError(error);
    }


    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    // Indicates that no ads are available to be displayed
    // This replaces the RewardedVideoAvailabilityChangedEvent(false) event
    void RewardedVideoOnAdUnavailable()
    {
        Debug.Log("Unavailable");
    }

    // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Closed");
    }

    // The user completed to watch the video, and should be rewarded.
    // The placement parameter will include the reward data.
    // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded");
    }

    // The rewarded video ad was failed to show.
    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        Debug.LogError(error);
        Debug.Log(adInfo);
    }

    
    public void ShowRewardedVideo()
    {
        // 本来はこのメソッドがコールバックになっているべきだが、今回は特に処理をしないのでそのまま
        IronSource.Agent.showRewardedVideo();
    }
}
