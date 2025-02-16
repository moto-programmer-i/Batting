
public class SaveData
{
    /// <summary>
    /// 現在のバット
    /// </summary>
    public int CurrentBatIndex{ get; set; }

    public static SaveData CreateDefault() {
        SaveData saveData = new ();
        saveData.CurrentBatIndex = 0;
        return saveData;
    }
}
