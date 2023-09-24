using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileUtils
{
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

        using (StreamWriter sw = new StreamWriter(Path.Combine(directory, filename))) {
            sw.WriteLine(JsonUtility.ToJson(json));
        }
    }
}
