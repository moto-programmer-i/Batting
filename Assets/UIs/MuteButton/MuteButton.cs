using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MuteButton : VisualElement
{
    const string MUTED_TEXT = "\uf6a9";
    const string NOT_MUTED_TEXT = "\uf028";
    const string CLASS_NAME = "mute-button";

    const string CIRCLE_BUTTON_CLASS_NAME = "circle-button";
    public new class UxmlFactory : UxmlFactory<MuteButton, UxmlTraits> {}

    public bool IsMuted {private set; get;}

    private Button notMutedButton;
    private Button mutedButton;

    /// <summary>
    /// 閉じた後の処理
    /// </summary>
    public Action<bool> Muted;   
    
    public MuteButton()
    {
        // なぜかクラスはここでつけなければいけない、なぜUI Builderでつけたクラスが反映されないのか
        AddToClassList(CLASS_NAME);

        RegisterCallback<ClickEvent>(OnClicked);
        

        // UI Builderで作成したボタンは反映されない。意味不明だが、スクリプトで指定する必要がある
        mutedButton = new Button();
        mutedButton.AddToClassList(CIRCLE_BUTTON_CLASS_NAME);
        mutedButton.text = MUTED_TEXT;
        Add(mutedButton);

        notMutedButton = new Button();
        notMutedButton.AddToClassList(CIRCLE_BUTTON_CLASS_NAME);
        notMutedButton.text = NOT_MUTED_TEXT;
        Add(notMutedButton);
    }

    

    /// <summary>
    /// クリック時はミュートのON/OFF
    /// </summary>
    /// <param name="evt"></param>
    private void OnClicked(ClickEvent evt)
    {
        // Only perform this action at the target, not in a parent
        if (evt.propagationPhase != PropagationPhase.AtTarget){
            return;
        }

        Mute(!IsMuted);
    }

    public void Mute(bool isMuted)
    {
        IsMuted = isMuted;
        
        // ボタンの表示切り替え
        if(IsMuted) {
            mutedButton.style.display = DisplayStyle.Flex;
            notMutedButton.style.display = DisplayStyle.None;
        }
        else {
            mutedButton.style.display = DisplayStyle.None;
            notMutedButton.style.display = DisplayStyle.Flex;
        }

        // ミュートの処理を実行
        Muted?.Invoke(IsMuted);
    }
}
