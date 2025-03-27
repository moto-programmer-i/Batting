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
        try {
            SaveData = FileUtils.LoadJson<SaveData>(SAVEDATA_FILENAME);

            // セーブデータがnullになってしまった場合に備えて、一応初期化する
            SaveData.InitNullObjects();
        }
        // ファイルがなければログ出力
        catch(FileNotFoundException e) {
            Debug.Log(e.Message);
        }

        // セーブデータがなければ作成
        if (SaveData == null) {
            SaveData = SaveData.CreateDefault();

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
    /// セーブする
    /// </summary>
    /// <returns></returns>
    public void Save()
    {
        // メインスレッド以外でget_persistentDataPathが呼べないので、非同期は無理
        // await Task.Run(() => FileUtils.SaveJson(SaveData, SAVEDATA_FILENAME));

        // 毎回ファイルを書くのではなく、ゲーム終了時にするべきかは要検討
        
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
}
