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
    private List<BatSetting> BatSettings = new();

    [SerializeField]
    private UIDocument BatSelector;

    [SerializeField]
    private GameObject CurrentBatParent;

    private Button batListDisplayButton;

    private CloseArea closeArea;
    /// <summary>
    /// バット選択画面
    /// </summary>
    private ScrollView batListView;

    private int selectedBatIndex = 0;

    void Awake()
    {
        // バットリスト表示ボタン初期化
        batListDisplayButton = BatSelector.rootVisualElement.Q<Button>(BatListDisplayButtonName);
        batListDisplayButton.clicked += () => ShowBatList(true);

        // 閉じる用の画面初期化
        closeArea = BatSelector.rootVisualElement.Q<CloseArea>();
        closeArea.Closed = () => ShowBatList(false);
        // 非表示でもなぜかデフォルトがFlexになっているので、Noneにしておく
        closeArea.style.display = DisplayStyle.None;

        // バットリスト初期化
        batListView = BatSelector.rootVisualElement.Q<ScrollView>();
        

        var radioButtonGroup = batListView.Q<RadioButtonGroup>();
        radioButtonGroup.choices = GetBatNames();
        selectedBatIndex = radioButtonGroup.value = 0;

        radioButtonGroup.RegisterCallback<ChangeEvent<int>>((e) =>
        {
            selectedBatIndex = e.newValue;

            // 選択されたらリスト非表示
            ShowBatList(false);
        });

        // ラジオボタンの設定
        var radioButtons = radioButtonGroup.Children();
        for (int i = 0; i < BatSettings.Count; ++i) {
            var radioButton = radioButtons.ElementAt(i) as RadioButton;
            radioButton.style.backgroundImage = BatSettings[i].Icon;
        }
    }

    public List<BatSetting> GetBatSettings()
    {
        return BatSettings;
    }

    /// <summary>
    /// バット選択画面の表示切り替え
    /// </summary>
    // public void ToggleBatSelector()
    // {        
    //     switch(closeArea.style.display.value) {
    //         case DisplayStyle.None:
    //             closeArea.style.display = DisplayStyle.Flex;
    //             batListDisplayButton.style.display = DisplayStyle.None;
    //         break;

    //         // デフォルトはバット選択画面非表示
    //         default:
    //             closeArea.style.display = DisplayStyle.None;
    //             batListDisplayButton.style.display = DisplayStyle.Flex;
    //         break;
    //     }
    // }

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
        return BatSettings.ConvertAll(e => e.Name);
    }

    public BatSetting GetCurrentBatSetting()
    {
        return BatSettings[selectedBatIndex];
    }
    


}
