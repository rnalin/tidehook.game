using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] XPSystem xpSystem;
    [SerializeField] CurrencySystem currencySystem;
    [SerializeField] FishingController fishingController;
    [SerializeField] HouseController houseController;

    public XPSystem XP => xpSystem;
    public CurrencySystem Currency => currencySystem;

    SaveData saveData;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        saveData = SaveSystem.Load();
        xpSystem.Initialize(saveData);
        currencySystem.Initialize(saveData);
        houseController.Initialize(saveData);
        fishingController.SetActiveBait(saveData.activeBaitIndex);
    }

    void OnEnable() => FishingController.OnFishCaught += HandleFishCaught;
    void OnDisable() => FishingController.OnFishCaught -= HandleFishCaught;

    void HandleFishCaught(FishData fish)
    {
        xpSystem.AddXP(fish.xpReward);
        currencySystem.AddCoins(fish.coinReward);
    }

    void OnApplicationPause(bool paused) { if (paused) Save(); }
    void OnApplicationQuit() => Save();

    void Save()
    {
        xpSystem.WriteTo(saveData);
        currencySystem.WriteTo(saveData);
        houseController.WriteTo(saveData);
        saveData.activeBaitIndex = fishingController.ActiveBaitIndex;
        SaveSystem.Save(saveData);
    }
}
