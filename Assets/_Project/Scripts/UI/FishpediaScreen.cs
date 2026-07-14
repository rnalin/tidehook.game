using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishpediaScreen : MonoBehaviour
{
    [Header("Grid (Left Page)")]
    [SerializeField] Transform gridParent;
    [SerializeField] GameObject fishSlotPrefab;
    [SerializeField] int fishPerPage = 9;

    [Header("Detail (Right Page)")]
    [SerializeField] TextMeshProUGUI fishNameText;
    [SerializeField] Image fishDetailImage;
    [SerializeField] TextMeshProUGUI rarityText;
    [SerializeField] Image rarityBadgeBg;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Image locationImage;
    [SerializeField] GameObject locationRow;
    [SerializeField] GameObject detailPanel;

    [Header("Pagination")]
    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;
    [SerializeField] TextMeshProUGUI pageText;

    static readonly Color[] RarityColors =
    {
        new Color(0.55f, 0.55f, 0.55f), // Common
        new Color(0.20f, 0.70f, 0.20f), // Uncommon
        new Color(0.20f, 0.45f, 0.90f), // Rare
        new Color(0.65f, 0.20f, 0.90f), // Epic
        new Color(0.95f, 0.70f, 0.10f), // Legendary
    };

    readonly List<FishpediaFishSlot> slots = new();
    FishpediaFishSlot selectedSlot;
    int currentPage;
    int totalPages;

    void OnEnable()
    {
        InventorySystem.OnInventoryChanged += RefreshGrid;
        RefreshGrid();
    }

    void OnDisable() => InventorySystem.OnInventoryChanged -= RefreshGrid;

    void Start()
    {
        prevButton?.onClick.AddListener(PrevPage);
        nextButton?.onClick.AddListener(NextPage);
        BuildSlots();
        if (detailPanel != null) detailPanel.SetActive(false);
    }

    void BuildSlots()
    {
        foreach (Transform t in gridParent) Destroy(t.gameObject);
        slots.Clear();

        for (int i = 0; i < fishPerPage; i++)
        {
            var go = Instantiate(fishSlotPrefab, gridParent);
            slots.Add(go.GetComponent<FishpediaFishSlot>());
        }
        RefreshGrid();
    }

    public void RefreshGrid()
    {
        if (GameManager.Instance == null || slots.Count == 0) return;
        var inv = GameManager.Instance.Inventory;
        int total = inv.FishCount;
        totalPages = Mathf.Max(1, Mathf.CeilToInt((float)total / fishPerPage));
        currentPage = Mathf.Clamp(currentPage, 0, totalPages - 1);

        for (int i = 0; i < slots.Count; i++)
        {
            int fishIndex = currentPage * fishPerPage + i;
            if (fishIndex < total)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].Setup(inv.GetFish(fishIndex), inv.IsDiscovered(fishIndex), SelectFish);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }

        if (pageText != null) pageText.text = $"{currentPage + 1}/{totalPages}";
        if (prevButton != null) prevButton.interactable = currentPage > 0;
        if (nextButton != null) nextButton.interactable = currentPage < totalPages - 1;
    }

    void SelectFish(FishData fish)
    {
        if (selectedSlot != null) selectedSlot.SetSelected(false);

        var inv = GameManager.Instance.Inventory;
        for (int i = 0; i < slots.Count; i++)
        {
            int fi = currentPage * fishPerPage + i;
            if (!slots[i].gameObject.activeSelf) continue;
            if (fi < inv.FishCount && inv.GetFish(fi) == fish)
            {
                selectedSlot = slots[i];
                slots[i].SetSelected(true);
                break;
            }
        }

        ShowDetail(fish);
    }

    void ShowDetail(FishData fish)
    {
        if (detailPanel != null) detailPanel.SetActive(true);

        var inv = GameManager.Instance.Inventory;
        bool discovered = false;
        for (int i = 0; i < inv.FishCount; i++)
            if (inv.GetFish(i) == fish) { discovered = inv.IsDiscovered(i); break; }

        if (fishNameText != null)
            fishNameText.text = discovered ? fish.fishName.ToUpper() : "???";

        if (fishDetailImage != null)
        {
            fishDetailImage.sprite = fish.sprite;
            fishDetailImage.color = discovered ? Color.white : new Color(0.15f, 0.08f, 0.04f);
        }

        Color rarityColor = RarityColors[(int)fish.rarity];
        if (rarityText != null) rarityText.text = fish.rarity.ToString().ToUpper();
        if (rarityBadgeBg != null) rarityBadgeBg.color = rarityColor;

        if (descriptionText != null)
            descriptionText.text = discovered ? fish.description : "Not yet discovered.";

        bool hasLocation = discovered && fish.locationSprite != null;
        if (locationRow != null) locationRow.SetActive(hasLocation);
        if (locationImage != null && hasLocation) locationImage.sprite = fish.locationSprite;
    }

    void PrevPage()
    {
        currentPage = Mathf.Max(0, currentPage - 1);
        RefreshGrid();
    }

    void NextPage()
    {
        currentPage = Mathf.Min(totalPages - 1, currentPage + 1);
        RefreshGrid();
    }
}
