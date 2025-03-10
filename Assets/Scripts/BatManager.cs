using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BatManager : MonoBehaviour
{
    public const string BatListDisplayButtonName = "bat-list-display-button";
    
    [SerializeField]
    private List<BatSetting> batSettings = new();

    [SerializeField]
    private UIDocument batSelector;

    [SerializeField]
    private GameObject currentBatParent;

    private Button batListDisplayButton;

    private RadioButtonGroup radioButtonGroup;

    private CloseArea closeArea;
    /// <summary>
    /// バット選択画面
    /// </summary>
    private ScrollView batListView;

    [SerializeField]
    private SaveDataManager saveDataManager;

    [SerializeField]
    private BatController batController;

    /// <summary>
    /// バットの無効化
    /// </summary>
    private List<DisabledBatMask> masks = new();

    void Awake()
    {
        // バットリスト表示ボタン初期化
        batListDisplayButton = batSelector.rootVisualElement.Q<Button>(BatListDisplayButtonName);
        batListDisplayButton.clicked += () => ShowBatList(true);

        // 閉じる用の画面初期化
        closeArea = batSelector.rootVisualElement.Q<CloseArea>();
        closeArea.Closed = () => ShowBatList(false);
        // 非表示でもなぜかデフォルトがFlexになっているので、Noneにしておく
        closeArea.style.display = DisplayStyle.None;

        // バットリスト初期化
        InitBatList();

        
        // セーブデータから現在のバット設定を適用
        saveDataManager.AddAfterLoad((saveData) => {
            radioButtonGroup.value = saveDataManager.SaveData.CurrentBatIndex;
            ChangeBat(GetCurrentBatSetting());
        });
        
    }

    public void InitBatList()
    {
        batListView = batSelector.rootVisualElement.Q<ScrollView>();
        radioButtonGroup = batListView.Q<RadioButtonGroup>();
        radioButtonGroup.choices = GetBatNames();
        radioButtonGroup.value = 0;

        radioButtonGroup.RegisterCallback<ChangeEvent<int>>((e) =>
        {
            radioButtonGroup.value = e.newValue;

            // 選択されたらリスト非表示
            ShowBatList(false);

            ChangeBat(GetCurrentBatSetting());
        });

        // ラジオボタンの設定
        var radioButtons = radioButtonGroup.Children();
        for (int i = 0; i < batSettings.Count; ++i) {
            var radioButton = radioButtons.ElementAt(i) as RadioButton;
            radioButton.style.backgroundImage = batSettings[i].Icon;
            radioButton.style.color = batSettings[i].LabelColor;
            radioButton.style.unityTextOutlineColor = batSettings[i].LabelOutlineColor;
            
            // 最大飛距離が足りないなら使えなくしておく
            var mask = new DisabledBatMask(radioButton, batSettings[i].EnableMeter);
            saveDataManager.AddAfterLoad(savedata => {
                mask.EnableByMeter(saveDataManager.SaveData.MaxMeter);
            });
            
            masks.Add(mask);
        }
        batController.OnMaxMeterChange.Add(meter => masks.ForEach(mask => mask.EnableByMeter(meter)));
    }


    public List<BatSetting> GetBatSettings()
    {
        return batSettings;
    }


    public void ShowBatList(bool visible)
    {        
        if (visible) {
            closeArea.style.display = DisplayStyle.Flex;
            batListDisplayButton.style.display = DisplayStyle.None;
        }
        else {
            closeArea.style.display = DisplayStyle.None;
            batListDisplayButton.style.display = DisplayStyle.Flex;
        }
    }

    public List<string> GetBatNames()
    {
        return batSettings.ConvertAll(e => e.Name);
    }

    public BatSetting GetCurrentBatSetting()
    {
        return batSettings[radioButtonGroup.value];
    }

    public void ChangeBat(BatSetting setting)
    {
        if (setting == null) {
            return;
        }

        // 現在のバットを消す（一応全て消す）
        for (int i = currentBatParent.transform.childCount - 1; i >= 0; --i) {
            Destroy(currentBatParent.transform.GetChild(i).gameObject);
        }

        // 新しいバットを追加、オブジェクトプールとかした方がいいのかとかは不明
        Instantiate(setting.Bat, currentBatParent.transform);
        batController.SetAmplifier(setting.Amplifier);


        // セーブデータ更新
        saveDataManager.SaveData.CurrentBatIndex = radioButtonGroup.value;
        saveDataManager.Save();
    }

    void OnChangeMaxMeter(float maxMeter) {
        masks.ForEach(mask => mask.EnableByMeter(maxMeter));
    }
    


}
