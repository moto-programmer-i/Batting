using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class CloseArea : VisualElement
{
    const string CLASS_NAME = "close-area";
    public new class UxmlFactory : UxmlFactory<CloseArea> {}

    /// <summary>
    /// 閉じた後の処理
    /// </summary>
    public Action Closed;
    
    
    public CloseArea()
    {
        // なぜかクラスはここでつけなければいけない、なぜUI Builderでつけたクラスが反映されないのか
        AddToClassList(CLASS_NAME);

        RegisterCallback<ClickEvent>(OnClicked);        
    }

    

    /// <summary>
    /// クリック時は閉じる
    /// </summary>
    /// <param name="evt"></param>
    private void OnClicked(ClickEvent evt)
    {
        // Only perform this action at the target, not in a parent
        if (evt.propagationPhase != PropagationPhase.AtTarget){
            return;
        }
        

        style.display = DisplayStyle.None;

        // 閉じた後の処理を実行
        Closed?.Invoke();
    }

    public void Show(bool visible)
    {
        if (visible) {
            style.display = DisplayStyle.Flex;
            return;
        }
        style.display = DisplayStyle.None;
    }
}
