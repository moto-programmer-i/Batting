using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class ConfigManager : MonoBehaviour
{
    // UI Builderの名前と対応させる必要がある
    const string MASTER_NAME = "master";
    const string BGM_NAME = "bgm";
    const string SE_NAME = "se";
    const string CONFIG_BUTTON_NAME = "config-button";

    const string THIRD_PARTY_NAME  = "third-party";

    // AudioMixerの名前と対応させる必要がある
    const string AUDIO_GROUP_MASTER_NAME = "Master";
    const string AUDIO_GROUP_BGM_NAME = "BGM";
    const string AUDIO_GROUP_SE_NAME = "SE";

    
    [SerializeField]
    private UIDocument home;
    
    [SerializeField]
    private UIDocument config;

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private AudioSource bgmSource;

    [SerializeField]
    private SaveDataManager saveDataManager;

    private Button configButton;

    private CloseArea configArea;

    private AudioContainer master;
    private AudioContainer bgm;
    private AudioContainer se;

    private Label thirdParty;

    [SerializeField]
    private string thirdPartyLink;
    
    void Awake()
    {        
        // 表示切り替えの設定
        configButton = home.rootVisualElement.Q<Button>(CONFIG_BUTTON_NAME);
        configArea = config.rootVisualElement.Q<CloseArea>();
        configButton.RegisterCallback<ClickEvent>(evt => configArea.Show(true));
        configArea.Closed = () => {
            // 設定画面を閉じたときに保存する
            master.SetVolumeData(saveDataManager.SaveData.Master);
            bgm.SetVolumeData(saveDataManager.SaveData.Bgm);
            se.SetVolumeData(saveDataManager.SaveData.Se);
            saveDataManager.Save();
        };
        
        // 初期化
        master = CreateContainerFrom(AUDIO_GROUP_MASTER_NAME, config.rootVisualElement.Q<VisualElement>(MASTER_NAME));
        bgm = CreateContainerFrom(AUDIO_GROUP_BGM_NAME, config.rootVisualElement.Q<VisualElement>(BGM_NAME));
        se = CreateContainerFrom(AUDIO_GROUP_SE_NAME, config.rootVisualElement.Q<VisualElement>(SE_NAME));

        // なぜか<a>がそのまま動かないので、無理やりやる
        thirdParty = config.rootVisualElement.Q<Label>(THIRD_PARTY_NAME);
        thirdParty.RegisterCallback<ClickEvent>(e => {
            Application.OpenURL(thirdPartyLink);
        });
        
    }

    void Start()
    {
        // セーブデータ読み込み
        // （AudioMixerがStartのタイミングの必要がある）
        // https://docs.unity3d.com/ja/2022.3/ScriptReference/Audio.AudioMixer.SetFloat.html
        saveDataManager.AddAfterLoad(saveData => {
            master.InitFromVolumeData(saveData.Master);
            bgm.InitFromVolumeData(saveData.Bgm);
            se.InitFromVolumeData(saveData.Se);

            // 音量設定後、BGMの再生を始める
            bgmSource.Play();
        });
    }

    private AudioContainer CreateContainerFrom(string mixerName, VisualElement panel)
    {
        return new AudioContainer(
            mixerName,
            panel.Q<MuteButton>(),
            panel.Q<Slider>(),
            audioMixer);
    }



    
}
