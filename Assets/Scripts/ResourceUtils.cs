using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class ResourceUtils
{
    /// <summary>
    /// Jsonファイルからインスタンス作成
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filepath">Resources以下のパス（拡張子は不要）</param>
    /// <returns></returns>
    public static T LoadJson<T>(string filepath)
    {
        // resourcesのパスはビルドするとなくなるらしく、Loadで取得する必要があるらしい
        string inputJson = Resources.Load<TextAsset>(filepath).ToString();
        return JsonConvert.DeserializeObject<T>(inputJson, FileUtils.JSON_SETTINGS);
    }
}
