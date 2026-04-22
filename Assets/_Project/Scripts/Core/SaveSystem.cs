using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data) =>
        File.WriteAllText(FilePath, JsonUtility.ToJson(data));

    public static SaveData Load()
    {
        if (!File.Exists(FilePath)) return new SaveData();
        try { return JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath)); }
        catch { return new SaveData(); }
    }
}

[Serializable]
public class SaveData
{
    public int coins = 0;
    public int xp = 0;
    public int level = 1;
    public int activeBaitIndex = 0;
    public int[] purchasedTiers = new int[0];
}
