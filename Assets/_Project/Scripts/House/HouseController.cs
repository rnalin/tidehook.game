using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HouseController : MonoBehaviour
{
    public static event Action<int> OnUpgradePurchased; // tier index

    [SerializeField] UpgradeData[] upgrades;   // T1→T5, assign in Inspector
    [SerializeField] GameObject[] houseTiers;  // one GameObject per tier

    HashSet<int> purchased = new();

    public int UpgradeCount => upgrades.Length;

    public void Initialize(SaveData data)
    {
        purchased = new HashSet<int>(data.purchasedTiers ?? Array.Empty<int>());
        RefreshVisuals();
    }

    public void WriteTo(SaveData data) =>
        data.purchasedTiers = purchased.ToArray();

    public bool TryPurchase(int index)
    {
        if (index < 0 || index >= upgrades.Length) return false;
        if (purchased.Contains(index)) return false;

        var upgrade = upgrades[index];
        if (GameManager.Instance.XP.Level < upgrade.requiredLevel) return false;
        if (!GameManager.Instance.Currency.SpendCoins(upgrade.cost)) return false;

        purchased.Add(index);
        RefreshVisuals();
        OnUpgradePurchased?.Invoke(index);
        return true;
    }

    public bool IsPurchased(int index) => purchased.Contains(index);

    public bool IsAvailable(int index)
    {
        if (index < 0 || index >= upgrades.Length) return false;
        var u = upgrades[index];
        return GameManager.Instance.XP.Level >= u.requiredLevel &&
               GameManager.Instance.Currency.Coins >= u.cost &&
               !purchased.Contains(index);
    }

    public UpgradeData GetUpgrade(int index) => upgrades[index];

    void RefreshVisuals()
    {
        for (int i = 0; i < houseTiers.Length; i++)
            if (houseTiers[i] != null)
                houseTiers[i].SetActive(purchased.Contains(i));
    }
}
