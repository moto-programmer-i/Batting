using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextSettings : MonoBehaviour
{
    /// <summary>
    /// 飛距離が表示される時間
    /// </summary>
    [field: SerializeField]
    public int DistanceDisplayMiliSeconds { get; private set;} = 1000;
    
    [SerializeField]
    private List<TextSetting> textSettings = new ();

    // /// <summary>
    // /// 設定マップ
    // /// </summary>
    // public Dictionary<string, TextSetting> SettingMap { get; private set;}
    
    public TextSettings()
    {
        // 自分で管理クラスにインスタンスを設定。こうすべきかは不明。
        SettingsManager.TextSettings = this;
    }

    public void Awake() 
    {
        // 飛距離の昇順にソート
        textSettings.Sort((a,b) => a.Distance - b.Distance);
        
        
        
        // 初期化
        // SettingMap = textSettings.ToDictionary(
        //     e => e.Key,
        //     e => e);
    }

    /// <summary>
    /// 飛距離からテキストの設定を返す
    /// </summary>
    /// <param name="distance"></param>
    /// <returns>飛距離以下の最も大きい設定</returns>
    public TextSetting fromDistance(int distance) {
        return textSettings.FindLast(e => e.Distance < distance);
    }
}
