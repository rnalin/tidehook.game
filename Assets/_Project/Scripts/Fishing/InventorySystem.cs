using System;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static event Action OnInventoryChanged;

    [SerializeField] FishData[] catalog;

    int[] counts;

    public int FishCount => catalog?.Length ?? 0;
    public FishData GetFish(int i) => catalog[i];
    public int GetCount(int i) => (counts != null && i < counts.Length) ? counts[i] : 0;
    public bool IsDiscovered(int i) => GetCount(i) > 0;

    public void Initialize(SaveData data)
    {
        counts = new int[catalog.Length];
        if (data.fishCounts != null)
            for (int i = 0; i < Mathf.Min(data.fishCounts.Length, counts.Length); i++)
                counts[i] = data.fishCounts[i];
    }

    public void WriteTo(SaveData data)
    {
        data.fishCounts = (int[])counts.Clone();
    }

    public void AddFish(FishData fish)
    {
        int i = Array.IndexOf(catalog, fish);
        if (i < 0) return;
        counts[i]++;
        OnInventoryChanged?.Invoke();
    }
}
