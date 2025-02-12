using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BatManager : MonoBehaviour
{
    [SerializeField]
    private List<BatSetting> BatSettings = new();

    [SerializeField]
    private UIDocument BatSelector;
    void Awake()
    {
        var listView = BatSelector.rootVisualElement.Q<ListView>();
        listView.itemsSource = BatSettings;

        // リストの要素作成時
        listView.makeItem = () => {
            var button = new RadioButton();
            button.AddToClassList(BatSetting.ButtonClassName);
            return button;
        };

        // 要素の対応
        listView.bindItem = (e, i) => {
            RadioButton button = e as RadioButton;
            button.label = BatSettings[i].Name;
            button.style.backgroundImage = BatSettings[i].Icon;
        };

    }

    public List<BatSetting> GetBatSettings()
    {
        return BatSettings;
    }


}
