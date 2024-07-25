using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextSettingsList : MonoBehaviour
{
    [SerializeField]
    private List<TextSetting> textSettings = new ();

    
    // 内部クラスにしないと、Inspector上で中身が見えない。なんで？
    [System.Serializable] 
    public class TextSetting {
        // fieldをつけないとInspector上で中身が見えない
        // 参考 https://waken.hatenablog.com/entry/2022/03/17/151205
        [field: SerializeField]
        public TextSettingEnum Key { get; set; }


        [field: SerializeField]
        public int Size { get; set; }


        [field: SerializeField]
        public Color Color { get; set; }
    }

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
