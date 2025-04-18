
using UnityEngine.PlayerLoop;

public class SaveData
{
    /// <summary>
    /// 現在のバット
    /// </summary>
    public int CurrentBatIndex{ get; set; }

    public float MaxMeter {get; set;}

    public VolumeData Master {get; set;}
    public VolumeData Bgm {get; set;}
    public VolumeData Se {get; set;}

    public int DevelopmentMessageIndex {get; set;}

    public static SaveData CreateDefault() {
        SaveData saveData = new ();
        saveData.CurrentBatIndex = 0;
        saveData.MaxMeter = 0;
        saveData.DevelopmentMessageIndex = 0;
        saveData.InitNullObjects();
        return saveData;
    }

    /// <summary>
    /// nullなオブジェクトを初期化
    /// </summary>
    public void InitNullObjects()
    {
        if (Master == null) {
            Master = new ();
        }
        if (Bgm == null) {
            Bgm = new ();
        }
        if (Se == null) {
            Se = new ();
        }
    }
}
