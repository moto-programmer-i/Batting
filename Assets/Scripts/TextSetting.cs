using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SerializableがないとInspectorに表示されない
// MonoBehaviourを継承するとリストにいれたとき、Inspectorに表示されない
[System.Serializable]
public class TextSetting
{
    // fieldをつけないとInspector上で中身が見えない
    // 参考 https://waken.hatenablog.com/entry/2022/03/17/151205
    // [field: SerializeField]
    // public string Key { get; set; }

    [field: SerializeField]
    public int Distance { get; set; }


    [field: SerializeField]
    public int Size { get; set; }


    [field: SerializeField]
    public Color Color { get; set; }

    [field: SerializeField]
    public Color OutlineColor { get; set; }

    
    [field: SerializeField]
    public TMPro.FontWeight FontWeight { get; set; }
}
