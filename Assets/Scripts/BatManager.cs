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

    /// <summary>
    /// バット選択画面
    /// </summary>
    private ScrollView batListView;

    private int selectedBatIndex = 0;

    void Awake()
    {
        // バットリスト表示ボタン初期化
        batListDisplayButton = BatSelector.rootVisualElement.Q<Button>(BatListDisplayButtonName);
        batListDisplayButton.clicked += ToggleBatSelector;



        // バットリスト初期化
        batListView = BatSelector.rootVisualElement.Q<ScrollView>();
        // 非表示でもなぜかデフォルトがFlexになっているので、Noneにしておく
        batListView.style.display = DisplayStyle.None;

        var radioButtonGroup = batListView.Q<RadioButtonGroup>();
        radioButtonGroup.choices = GetBatNames();
        selectedBatIndex = radioButtonGroup.value = 0;

        radioButtonGroup.RegisterCallback<ChangeEvent<int>>((e) =>
        {
            selectedBatIndex = e.newValue;

            // 選択されたらリスト非表示
            ToggleBatSelector();
        });

        // ラジオボタンの設定
        var radioButtons = radioButtonGroup.Children();
        for (int i = 0; i < BatSettings.Count; ++i) {
            var radioButton = radioButtons.ElementAt(i) as RadioButton;
            radioButton.style.backgroundImage = BatSettings[i].Icon;
        }

// // Create a new field, disable it, and give it a style class.
// var csharpField = new RadioButtonGroup("C# Field", choices);
// csharpField.value = 0;
// csharpField.SetEnabled(false);
// csharpField.AddToClassList("some-styled-field");
// csharpField.value = uxmlField.value;
// container.Add(csharpField);

// // Mirror the value of the UXML field into the C# field.
// uxmlField.RegisterCallback<ChangeEvent<int>>((evt) =>
// {
//     csharpField.value = evt.newValue;
// });



        // // バットリスト初期化
        // batListView = BatSelector.rootVisualElement.Q<ListView>();
        // batListView.itemsSource = BatSettings;

        // // 非表示でもなぜかデフォルトがFlexになっているので、Noneにしておく
        // batListView.style.display = DisplayStyle.None;

        // // リストの要素作成時
        // batListView.makeItem = () => {
        //     var button = new RadioButton();
        //     button.AddToClassList(BatSetting.ButtonClassName);

        //     button.RegisterCallback<ChangeEvent<bool>>((evt) =>
        //     {
        //         Debug.Log("select index " + batListView.selectedIndex);
        //     });
            


        //     // button.RegisterCallback<ChangeEvent<bool>>((evt) =>
        //     // {
        //     //     // index を探す
        //     //     // Debug.Log(button.parent.ToString());
        //     //     // ScrollView view = button.parent as ScrollView;
        //     //     Debug.Log("バットindexof " + batListView.IndexOf(button));
        //     //     batListView.Children().
                
                
        //     // });

        //     return button;
        // };

        // // 要素が表示される旅に呼び出される
        // batListView.bindItem = (e, i) => {
        //     RadioButton button = e as RadioButton;
        //     button.label = BatSettings[i].Name;
        //     button.style.backgroundImage = BatSettings[i].Icon;

        //     // bindItemは要素が表示されるたびに呼び出されるので、イベントの登録は行ってはいけないらしい
        //     // https://qiita.com/AtsuAtsu0120/items/65c02fc2ce8cd10d8fda
        // };

        // // バット選択時に画面を消す
        // batListView.selectedIndicesChanged += (nums) => {ToggleBatSelector();};

        // // ダブルクリック時にも選択画面を消す
        // batListView.itemsChosen += (nums) => {ToggleBatSelector();};
    }

    public List<BatSetting> GetBatSettings()
    {
        return BatSettings;
    }

    /// <summary>
    /// バット選択画面の表示切り替え
    /// </summary>
    public void ToggleBatSelector()
    {        
        switch(batListView.style.display.value) {
            case DisplayStyle.None:
                batListView.style.display = DisplayStyle.Flex;
                batListDisplayButton.style.display = DisplayStyle.None;
            break;

            // デフォルトはバット選択画面非表示
            default:
                batListView.style.display = DisplayStyle.None;
                batListDisplayButton.style.display = DisplayStyle.Flex;
            break;
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
