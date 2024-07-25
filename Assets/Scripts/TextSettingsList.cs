using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextSettingsList : MonoBehaviour
{
    [SerializeField]
    private List<TextSetting> textSettings = new ();

    /// <summary>
    /// 設定マップ
    /// </summary>
    public Dictionary<TextSettingEnum, TextSetting> SettingMap { get => settingMap;}
    private Dictionary<TextSettingEnum, TextSetting> settingMap;
    

    public void Awake() 
    {
        // 初期化
        settingMap = textSettings.ToDictionary(
            e => e.Key,
            e => e);
    }
}
