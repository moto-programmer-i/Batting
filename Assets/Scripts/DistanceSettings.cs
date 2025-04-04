using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DistanceSettings : MonoBehaviour
{
    /// <summary>
    /// 飛距離が表示される時間
    /// </summary>
    [field: SerializeField]
    public int DistanceDisplaySeconds { get; private set;} = 1;
    
    [SerializeField]
    private List<DistanceSetting> textSettings = new ();

    // /// <summary>
    // /// 設定マップ
    // /// </summary>
    // public Dictionary<string, TextSetting> SettingMap { get; private set;}
    
    public DistanceSettings()
    {
        // 自分で管理クラスにインスタンスを設定。こうすべきかは不明。
        SettingsManager.DistanceSettings = this;
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
    public DistanceSetting fromDistance(int distance) {
        return textSettings.FindLast(e => e.Distance < distance);
    }
}
