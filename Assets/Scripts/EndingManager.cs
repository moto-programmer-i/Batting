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
    private int delaySeconds = 1;

    private bool canPlay = true;

    [SerializeField]
    private AudioSource endingAudio;

    [SerializeField]
    private AudioSource mainAudio;

    private bool initialized = false;
    
    void Awake()
    {
    }

    /// <summary>
    /// 事前にui.enabled = trueが必要、この構成でないとWebGLで動作しない
    /// </summary>
    // ArgumentNullException: Value cannot be null.
    // Parameter name: e
    //   at UnityEngine.UIElements.UQueryExtensions.Q (UnityEngine.UIElements.VisualElement e, System.String name, System.String className) [0x00000] in <00000000000000000000000000000000>:0 
    public void Init()
    {
        if (initialized){
            return;
        }

        ending = ui.rootVisualElement.Q(ENDING_NAME);
        ending.EnableInClassList(SCROLL_CLASS, false);
        endingImage = ui.rootVisualElement.Q(ENDING_IMAGE_NAME);
        

        // ダブルクリックでスキップ
        ending.RegisterCallback<ClickEvent>(e => {
            if(UIUtils.IsDoubleClick(e)) {
                SkipEnding();
            }
            e.StopImmediatePropagation();
        });

        // エンディングの画像クリックで戻る
        endingImage.RegisterCallback<ClickEvent>(e => {
            ui.enabled = false;
            endingAudio.Stop();
            mainAudio.UnPause();
        });

        canPlay = true;
    }

    // void Start()
    // {
    //     StartEnding();
    // }

    public void StartEnding()
    {
        if (!canPlay) {
            return;
        }

        ui.enabled = true;

        Init();

        mainAudio.Pause();
        
        // 指定時間後にエンディングを開始
        AsyncUtils.Delay(delaySeconds, () => {
            ending.EnableInClassList(ANIMATION_CLASS, true);

            // エンディングの高さが計算されてからスクロール
            ending.RegisterCallback<GeometryChangedEvent>(e => {
                ending.EnableInClassList(SCROLL_CLASS, true);
            });

            endingAudio.Play();
        });        

        // 一度エンディングに入ったら、そのプレイ中は流さない
        canPlay = false;
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
