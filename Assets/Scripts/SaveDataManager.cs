using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    const string SAVEDATA_FILENAME = "battingCenter.json";
    public SaveData SaveData {get; private set;}

    private List<Action<SaveData>> afterLoads = new ();
    
    // Start is called before the first frame update
    void Awake()
    {
        // セーブデータを読み込み
        // 非同期でやるべきかも？時間がかかる場合は検討
        try {
            SaveData = FileUtils.LoadJson<SaveData>(SAVEDATA_FILENAME);
        }
        // ファイルがなければログ出力
        catch(FileNotFoundException e) {
            Debug.Log(e.Message);
        }

        // セーブデータがなければ作成
        if (SaveData == null) {
            SaveData = SaveData.CreateDefault();

            // ファイル更新は非同期で行う（意味不明の書き方だが、これで合っているらしい）
            // _ = Save();
            Save();
        }

        // セーブデータ読み込み後の処理
        // Awakeメインスレッドで呼ばれるらしいので大丈夫なはずではある
        // https://light11.hatenadiary.com/entry/2024/12/18/190914
        foreach(Action<SaveData> afterLoad in afterLoads) {
            afterLoad.Invoke(SaveData);
        }
        afterLoads.Clear();
    }

    
    /// <summary>
    /// 非同期にセーブする
    /// </summary>
    /// <returns></returns>
    // public async Task Save()
    public void Save()
    {
        // await Task.Run(() => FileUtils.SaveJson(SaveData, SAVEDATA_FILENAME));
        FileUtils.SaveJson(SaveData, SAVEDATA_FILENAME);
    }

    public void AddAfterLoad(Action<SaveData> afterLoad)
    {
        // セーブデータがすでに読み込まれていれば実行
        if (SaveData != null) {
            afterLoad.Invoke(SaveData);
            return;
        }

        afterLoads.Add(afterLoad);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="maxMeter"></param>
    /// <returns>変更あり</returns>
    public bool UpdateMaxMeter(float maxMeter)
    {
        Debug.Log($"更新{SaveData.MaxMeter} -> maxMeter");
        if (maxMeter <= SaveData.MaxMeter) {
            return false;
        }
        Debug.Log("更新処理中・・・");
        SaveData.MaxMeter = maxMeter;

        // ファイル更新は非同期で行う（意味不明の書き方だが、これで合っているらしい）
        // _ = Save();
        Save();

        return true;
    }

   
}
