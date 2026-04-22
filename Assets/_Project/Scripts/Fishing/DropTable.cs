using UnityEngine;

public static class DropTable
{
    public static FishData Pick(BaitData bait)
    {
        float total = 0f;
        foreach (var entry in bait.dropTable) total += entry.weight;

        float roll = Random.Range(0f, total);
        foreach (var entry in bait.dropTable)
        {
            roll -= entry.weight;
            if (roll <= 0f) return entry.fish;
        }
        return bait.dropTable[^1].fish;
    }
}
