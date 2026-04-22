using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScreen : MonoBehaviour
{
    [SerializeField] GameObject screenRoot;
    [SerializeField] Transform listParent;
    [SerializeField] GameObject upgradeItemPrefab;
    [SerializeField] Button closeButton;
    [SerializeField] HouseController houseController;

    List<UpgradeItemUI> items = new();

    void Start()
    {
        closeButton.onClick.AddListener(() => screenRoot.SetActive(false));
        BuildList();
        screenRoot.SetActive(false);
    }

    void OnEnable()
    {
        HouseController.OnUpgradePurchased += HandleChange;
        CurrencySystem.OnCoinsChanged += HandleCoinsChange;
        XPSystem.OnLevelUp += HandleLevelChange;
    }

    void OnDisable()
    {
        HouseController.OnUpgradePurchased -= HandleChange;
        CurrencySystem.OnCoinsChanged -= HandleCoinsChange;
        XPSystem.OnLevelUp -= HandleLevelChange;
    }

    void HandleChange(int _) => RefreshAll();
    void HandleCoinsChange(int _) => RefreshAll();
    void HandleLevelChange(int _) => RefreshAll();

    public void Open() => screenRoot.SetActive(true);

    void BuildList()
    {
        for (int i = 0; i < houseController.UpgradeCount; i++)
        {
            int index = i;
            var go = Instantiate(upgradeItemPrefab, listParent);
            var item = go.GetComponent<UpgradeItemUI>();
            item.Setup(houseController.GetUpgrade(index), () => houseController.TryPurchase(index));
            items.Add(item);
        }
        RefreshAll();
    }

    void RefreshAll()
    {
        for (int i = 0; i < items.Count; i++)
            items[i].Refresh(houseController.IsPurchased(i), houseController.IsAvailable(i));
    }
}
