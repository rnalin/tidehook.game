using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaitCardUI : MonoBehaviour
{
    [SerializeField] Image baitIcon;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI lockText;
    [SerializeField] Button selectButton;
    [SerializeField] GameObject lockedOverlay;
    [SerializeField] GameObject activeIndicator;

    Action onSelect;

    public void Setup(BaitData data, Action selectCallback)
    {
        if (baitIcon != null) baitIcon.sprite = data.sprite;
        nameText.text = data.baitName;
        lockText.text = data.unlockLevel <= 1 ? "" : $"Nível {data.unlockLevel}";
        onSelect = selectCallback;
        selectButton.onClick.AddListener(() => onSelect?.Invoke());
    }

    public void Refresh(bool unlocked, bool isActive)
    {
        selectButton.interactable = unlocked;
        lockedOverlay?.SetActive(!unlocked);
        activeIndicator?.SetActive(isActive);
    }
}
