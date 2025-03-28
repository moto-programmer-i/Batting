using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class BatManager : MonoBehaviour
{
    public const string BAT_LIST_DISPLAY_BUTTON_NAME = "bat-list-display-button";
    public const string NEW_BAT_NOTICE_NAME = "new-bat-notice";

    /// <summary>
    /// スクロールバーの幅。取得が面倒そうなので定義。
    /// </summary>
    public const int SCROLL_BAR_WIDTH_PX = 24;

    /// <summary>
    /// スクロールせずに表示できるバットの数（目安、必須ではない）
    /// </summary>
    public const float BAT_NUM_IN_PAGE = 2f;

    [SerializeField]
    private List<BatSetting> batSettings = new();

    [SerializeField]
    private UIDocument home;

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

    private Label newBatNotice;

    [SerializeField]
    private SaveDataManager saveDataManager;

    [SerializeField]
    private BatController batController;

    [SerializeField]
    private DistanceManager distanceManager;

    /// <summary>
    /// バットの無効化
    /// </summary>
    private List<DisabledBatMask> masks = new();

    void Awake()
    {
        // バットリスト表示ボタン初期化
        batListDisplayButton = home.rootVisualElement.Q<Button>(BAT_LIST_DISPLAY_BUTTON_NAME);
        batListDisplayButton.clicked += () => {
            ShowBatList(true);
            ShowNewBatNotice(false);
        };
        newBatNotice = home.rootVisualElement.Q<Label>(NEW_BAT_NOTICE_NAME);
        ShowNewBatNotice(false);

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

        // ダブルクリック時は閉じる
        radioButtonGroup.RegisterCallback<ClickEvent>((e) =>
        {
            if (!UIUtils.IsDoubleClick(e)){
                return;
            }
            ShowBatList(false);
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

        // 画面サイズの変更に応じて、ラジオボタンのサイズを調整
        batListView.RegisterCallback<GeometryChangedEvent>(e => {
            var icon = batSettings.First().Icon;
            var height = icon.height *
                        (e.newRect.width - SCROLL_BAR_WIDTH_PX) / icon.width / BAT_NUM_IN_PAGE;

            foreach(var radioButton in radioButtons) {
                radioButton.style.height = height;
            }
        });

        // 飛距離に応じて新しいバットを入手
        distanceManager.OnMaxMeterChange.Add(meter => {
            masks.ForEach(mask => {
                // 入手していたら通知を出す
                bool wasEnabled = mask.IsEnabled();
                if(mask.EnableByMeter(meter) && !wasEnabled) {
                    ShowNewBatNotice(true);
                }
            });
        });
        
    }

    private void ShowNewBatNotice(bool visible)
    {
        newBatNotice.style.visibility = visible? Visibility.Visible: Visibility.Hidden;
    }


    public List<BatSetting> GetBatSettings()
    {
        return batSettings;
    }


    public void ShowBatList(bool visible)
    {        
        if (visible) {
            closeArea.style.display = DisplayStyle.Flex;
        }
        else {
            closeArea.style.display = DisplayStyle.None;
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
        batController.SetCurrentBat(setting);


        // セーブデータ更新
        saveDataManager.SaveData.CurrentBatIndex = radioButtonGroup.value;
        saveDataManager.Save();
    } 
}
