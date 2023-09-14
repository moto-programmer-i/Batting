using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceUtils
{
    /// <summary>
    /// Jsonファイルからインスタンス作成
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filepath">Resources以下のパス（拡張子は不要）</param>
    /// <returns></returns>
    public static T LoadJson<T>(string filepath){
        string inputJson = Resources.Load<TextAsset>(filepath).ToString();
        return JsonUtility.FromJson<T>(inputJson);
    }
}
