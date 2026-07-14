using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] GameObject panelRoot;
    [SerializeField] Transform gridParent;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Button closeButton;
    [SerializeField] int slotCount = 24;

    readonly List<InventorySlotUI> slots = new();

    void Start()
    {
        closeButton?.onClick.AddListener(Close);
        BuildSlots();
        panelRoot.SetActive(false);
    }

    void OnEnable() => InventorySystem.OnInventoryChanged += Refresh;
    void OnDisable() => InventorySystem.OnInventoryChanged -= Refresh;

    void BuildSlots()
    {
        foreach (Transform t in gridParent) Destroy(t.gameObject);
        slots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            var go = Instantiate(slotPrefab, gridParent);
            slots.Add(go.GetComponent<InventorySlotUI>());
        }
    }

    public void Open()
    {
        panelRoot.SetActive(true);
        Refresh();
    }

    public void Close() => panelRoot.SetActive(false);

    void Refresh()
    {
        if (GameManager.Instance == null) return;
        var inv = GameManager.Instance.Inventory;

        int slotIdx = 0;
        for (int i = 0; i < inv.FishCount && slotIdx < slots.Count; i++)
        {
            int count = inv.GetCount(i);
            if (count > 0)
                slots[slotIdx++].SetFish(inv.GetFish(i), count);
        }

        for (int i = slotIdx; i < slots.Count; i++)
            slots[i].SetEmpty();
    }
}
