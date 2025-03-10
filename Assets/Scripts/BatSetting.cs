using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SerializableがないとInspectorに表示されない
// MonoBehaviourを継承するとリストにいれたとき、Inspectorに表示されない
[System.Serializable]
public class BatSetting
{
    // .bat-buttonとすると対応しないので注意
    public const string ButtonClassName = "bat-button";

    // fieldをつけないとInspector上で中身が見えない
    // 参考 https://waken.hatenablog.com/entry/2022/03/17/151205
    // [field: SerializeField]
    // public string Key { get; set; }

    [field: SerializeField]
    public GameObject Bat { get; set; }

    
    [field: SerializeField]
    public Texture2D Icon { get; set; }

    [field: SerializeField]
    public string Name { get; set; }

    /// <summary>
    /// バットの飛距離の倍率
    /// </summary>
    [field: SerializeField]
    public float Amplifier { get; set; }

    /// <summary>
    /// 有効にするメートル
    /// </summary>
    [field: SerializeField]
    public int EnableMeter {get; set;}

    /// <summary>
    /// 文字の色
    /// </summary>
    [field: SerializeField]
    public Color LabelColor {get; set;}

    /// <summary>
    /// 文字の縁の色
    /// </summary>
    [field: SerializeField]
    public Color LabelOutlineColor {get; set;}
}
