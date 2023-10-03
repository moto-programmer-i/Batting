using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public static class FileUtils
{

    // JSON化の設定
    // 参考 https://qiita.com/matchyy/items/b1cd6f3a2a93749774da#tldr
    public static readonly JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public static string GetCurrentDirectory()
    {
        // 開発中は C:/Users/%USERNAME%/AppData/LocalLow/DefaultCompany/Batting
        return Application.persistentDataPath;
    }
    

    /// <summary>
    /// Jsonファイルを保存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filename">ファイル名</param>
    /// <param name="json"></param>
    /// /// <param name="directory">ディレクトリ（空の場合はカレントになる）</param>
    public static void SaveJson<T>(string filename, T json, string directory = "")
    {
        // ディレクトリの指定がない場合はカレント
        if (string.IsNullOrEmpty(directory)) {
            directory = GetCurrentDirectory();
        }

        // 参考
        // https://www.newtonsoft.com/json/help/html/SerializeWithJsonSerializerToFile.htm
        using (StreamWriter file = File.CreateText(Path.Combine(directory, filename)))
        {
            JsonSerializer serializer = JsonSerializer.CreateDefault(JSON_SETTINGS);
            serializer.Serialize(file, json);
        }
    }

    /// <summary>
    /// Jsonファイルを読み込み
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filename"></param>
    /// <param name="directory">ディレクトリ（空の場合はカレントになる）</param>
    /// <returns></returns>
    public static T LoadJson<T>(string filename, string directory = "")
    {
        // ディレクトリの指定がない場合はカレント
        if (string.IsNullOrEmpty(directory)) {
            directory = GetCurrentDirectory();
        }

        using (StreamReader file = File.OpenText(Path.Combine(directory, filename)))
        {
            JsonSerializer serializer = JsonSerializer.CreateDefault(JSON_SETTINGS);
            // なぜかジェネリックのメソッドが用意されてない
            return (T)serializer.Deserialize(file, typeof(T));
        }
    }
}
