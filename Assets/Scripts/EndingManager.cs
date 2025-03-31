using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class EndingManager : MonoBehaviour
{
    const string ENDING_NAME = "ending";
    const string ENDING_IMAGE_NAME = "ending-image";

    const string SCROLL_CLASS = "scroll";
    const string ANIMATION_CLASS = "animation";
    
    [SerializeField]
    private UIDocument ui;
    private VisualElement ending;
    private VisualElement endingImage;

    [field:SerializeField]
    public float MeterCondition {set; get;}

    [SerializeField]
    private int delayMiliSeconds = 1000;

    private bool enable = true;

    [SerializeField]
    private AudioSource endingAudio;

    [SerializeField]
    private AudioSource mainAudio;
    
    void Awake()
    {
        ending = ui.rootVisualElement.Q(ENDING_NAME);
        ending.EnableInClassList(SCROLL_CLASS, false);
        endingImage = ui.rootVisualElement.Q(ENDING_IMAGE_NAME);
        

        // ダブルクリックでスキップ
        ending.RegisterCallback<ClickEvent>(e => {
            if(UIUtils.IsDoubleClick(e)) {
                SkipEnding();
            }
        });

        // エンディングの画像クリックで戻る
        endingImage.RegisterCallback<ClickEvent>(e => {
            ui.enabled = false;
            endingAudio.Stop();
            mainAudio.UnPause();
        });

        enable = true;
    }

    void Start()
    {
        StartEnding();
    }

    public async void StartEnding()
    {
        if (!enable) {
            return;
        }

        mainAudio.Pause();
        
        // 指定時間後にエンディングを開始
        await Task.Delay(delayMiliSeconds);
        
        ui.enabled = true;
        ending.EnableInClassList(ANIMATION_CLASS, true);

        // エンディングの高さが計算されてからスクロール
        ending.RegisterCallback<GeometryChangedEvent>(e => {
            ending.EnableInClassList(SCROLL_CLASS, true);
        });

        endingAudio.Play();

        // 一度エンディングに入ったら、そのプレイ中は流さない
        enable = false;
    }

    public void SkipEnding()
    {
        ending.EnableInClassList(ANIMATION_CLASS, false);
    }

    /// <summary>
    /// エンディングの条件を満たすか
    /// </summary>
    /// <param name="meter"></param>
    /// <returns></returns>
    public bool ReachEnds(float meter) {
        return meter >= MeterCondition;
    }
}
