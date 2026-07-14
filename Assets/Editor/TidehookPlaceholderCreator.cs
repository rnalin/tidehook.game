using UnityEditor;
using UnityEngine;

public static class TidehookPlaceholderCreator
{
    [MenuItem("Tidehook/Create Placeholder Assets")]
    public static void CreatePlaceholders()
    {
        string folder = "Assets/_Project/ScriptableObjects";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets/_Project", "ScriptableObjects");
        }

        // Fish
        var fish1 = ScriptableObject.CreateInstance<FishData>();
        fish1.fishName = "Peixinho";
        fish1.rarity = Rarity.Common;
        fish1.xpReward = 5;
        fish1.coinReward = 5;
        AssetDatabase.CreateAsset(fish1, folder + "/Fish_Peixinho.asset");

        var fish2 = ScriptableObject.CreateInstance<FishData>();
        fish2.fishName = "Sardinha";
        fish2.rarity = Rarity.Uncommon;
        fish2.xpReward = 10;
        fish2.coinReward = 15;
        AssetDatabase.CreateAsset(fish2, folder + "/Fish_Sardinha.asset");

        var fish3 = ScriptableObject.CreateInstance<FishData>();
        fish3.fishName = "Dourado";
        fish3.rarity = Rarity.Rare;
        fish3.xpReward = 25;
        fish3.coinReward = 60;
        AssetDatabase.CreateAsset(fish3, folder + "/Fish_Dourado.asset");

        // Baits
        var bait1 = ScriptableObject.CreateInstance<BaitData>();
        bait1.baitName = "Isca Basica";
        bait1.unlockLevel = 1;
        bait1.dropTable = new FishDropEntry[]
        {
            new FishDropEntry(){ fish = fish1, weight = 55f },
            new FishDropEntry(){ fish = fish2, weight = 25f },
            new FishDropEntry(){ fish = fish3, weight = 20f },
        };
        AssetDatabase.CreateAsset(bait1, folder + "/Bait_Basica.asset");

        var bait2 = ScriptableObject.CreateInstance<BaitData>();
        bait2.baitName = "Isca Minhoca";
        bait2.unlockLevel = 2;
        bait2.dropTable = new FishDropEntry[]
        {
            new FishDropEntry(){ fish = fish1, weight = 45f },
            new FishDropEntry(){ fish = fish2, weight = 35f },
            new FishDropEntry(){ fish = fish3, weight = 20f },
        };
        AssetDatabase.CreateAsset(bait2, folder + "/Bait_Minhoca.asset");

        var bait3 = ScriptableObject.CreateInstance<BaitData>();
        bait3.baitName = "Isca Brilhante";
        bait3.unlockLevel = 3;
        bait3.dropTable = new FishDropEntry[]
        {
            new FishDropEntry(){ fish = fish1, weight = 30f },
            new FishDropEntry(){ fish = fish2, weight = 40f },
            new FishDropEntry(){ fish = fish3, weight = 30f },
        };
        AssetDatabase.CreateAsset(bait3, folder + "/Bait_Brilhante.asset");

        // Upgrades
        var up1 = ScriptableObject.CreateInstance<UpgradeData>();
        up1.upgradeName = "Cabana T1";
        up1.tier = 1;
        up1.cost = 0;
        up1.requiredLevel = 1;
        up1.description = "Cabana inicial";
        AssetDatabase.CreateAsset(up1, folder + "/Upgrade_T1.asset");

        var up2 = ScriptableObject.CreateInstance<UpgradeData>();
        up2.upgradeName = "Cabana T2";
        up2.tier = 2;
        up2.cost = 100;
        up2.requiredLevel = 2;
        up2.description = "Pequena ampliacao";
        AssetDatabase.CreateAsset(up2, folder + "/Upgrade_T2.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Tidehook placeholders created in " + folder);
    }
}
