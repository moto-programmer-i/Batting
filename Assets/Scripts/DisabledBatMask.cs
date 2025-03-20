using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// unity-disabledのopacityを1にしないと、スタイルがうまく適用されない
// https://discussions.unity.com/t/how-to-disable-interaction-of-a-visualelement-and-all-children/943945/4

public class DisabledBatMask : VisualElement
{
    public const string CLASS_NAME = "bat-disabled";
    public new class UxmlFactory : UxmlFactory<DisabledBatMask> {}

    /// <summary>
    /// 有効にするメートル
    /// </summary>
    public int EnableMeter {get; set;}    

    public DisabledBatMask()
    {
        AddToClassList(CLASS_NAME);
        RegisterCallback<ClickEvent>(e => {
            // クリックイベントを停止
            e.StopImmediatePropagation();
            e.PreventDefault();        
        });
    }

    public DisabledBatMask(RadioButton parent, int enableMeter): this()
    {
        parent.Add(this);
        EnableMeter = enableMeter;

        // 説明を追加
        Label label = new Label();
        label.text = $"飛距離{enableMeter}mで解放";
        Add(label);
    }

    /// <summary>
    /// 最大飛距離に応じて有効化
    /// </summary>
    /// <param name="maxMeter">最大飛距離</param>
    /// <returns>有効になったらtrue</returns>
    public bool EnableByMeter(float maxMeter)
    {        
        // 有効にする
        if (maxMeter >= EnableMeter) {
            style.display = DisplayStyle.None;
            parent.SetEnabled(true);
            return true;
        }

        // 無効にする
        style.display = DisplayStyle.Flex;
        parent.SetEnabled(false);
        return false;
    }

    public bool IsEnabled()
    {
        return parent.enabledSelf;
    }
}
