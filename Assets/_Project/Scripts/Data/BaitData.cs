using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Tidehook/Bait Data")]
public class BaitData : ScriptableObject
{
    public string baitName;
    public int unlockLevel;
    public Sprite sprite;
    public FishDropEntry[] dropTable;
}

[Serializable]
public class FishDropEntry
{
    public FishData fish;
    [Range(0f, 100f)] public float weight;
}
