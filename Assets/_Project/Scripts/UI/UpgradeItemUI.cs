using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] Button buyButton;
    [SerializeField] GameObject purchasedBadge;
    [SerializeField] CanvasGroup canvasGroup;

    Action onBuy;

    public void Setup(UpgradeData data, Action buyCallback)
    {
        nameText.text = data.upgradeName;
        costText.text = data.cost == 0 ? "Grátis" : data.cost.ToString("N0") + " moedas";
        onBuy = buyCallback;
        buyButton.onClick.AddListener(() => onBuy?.Invoke());
    }

    public void Refresh(bool isPurchased, bool isAvailable)
    {
        purchasedBadge?.SetActive(isPurchased);
        buyButton.interactable = !isPurchased && isAvailable;
        if (canvasGroup != null)
            canvasGroup.alpha = isPurchased || isAvailable ? 1f : 0.5f;
    }
}
