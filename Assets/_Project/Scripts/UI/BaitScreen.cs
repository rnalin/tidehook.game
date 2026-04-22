using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaitScreen : MonoBehaviour
{
    [SerializeField] GameObject screenRoot;
    [SerializeField] Transform listParent;
    [SerializeField] GameObject baitCardPrefab;
    [SerializeField] Button closeButton;
    [SerializeField] FishingController fishingController;
    [SerializeField] BaitData[] baits;

    List<BaitCardUI> cards = new();

    void Start()
    {
        closeButton.onClick.AddListener(() => screenRoot.SetActive(false));
        BuildCards();
        screenRoot.SetActive(false);
    }

    void OnEnable() => XPSystem.OnLevelUp += HandleLevelUp;
    void OnDisable() => XPSystem.OnLevelUp -= HandleLevelUp;

    void HandleLevelUp(int _) => RefreshCards();

    public void Open() => screenRoot.SetActive(true);

    void BuildCards()
    {
        for (int i = 0; i < baits.Length; i++)
        {
            int index = i;
            var go = Instantiate(baitCardPrefab, listParent);
            var card = go.GetComponent<BaitCardUI>();
            card.Setup(baits[index], () => SelectBait(index));
            cards.Add(card);
        }
        RefreshCards();
    }

    void SelectBait(int index)
    {
        fishingController.SetActiveBait(index);
        RefreshCards();
    }

    void RefreshCards()
    {
        int level = GameManager.Instance.XP.Level;
        for (int i = 0; i < cards.Count; i++)
        {
            bool unlocked = level >= baits[i].unlockLevel;
            bool active = fishingController.ActiveBaitIndex == i;
            cards[i].Refresh(unlocked, active);
        }
    }
}
