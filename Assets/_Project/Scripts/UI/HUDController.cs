using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Player Card")]
    [SerializeField] Image playerAvatar;
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider xpSlider;
    [SerializeField] TextMeshProUGUI xpText;
    [SerializeField] TextMeshProUGUI coinsText;

    [Header("Active Bait Slot")]
    [SerializeField] Image activeBaitIcon;
    [SerializeField] TextMeshProUGUI activeBaitCountText;
    [SerializeField] BaitData[] baits;

    [Header("Double XP (Ad)")]
    [SerializeField] GameObject doubleIndicator;
    [SerializeField] TextMeshProUGUI doubleTimerText;
    [SerializeField] Button doubleButton;


    [Header("References")]
    [SerializeField] FishingController fishingController;

    void OnEnable()
    {
        XPSystem.OnXPChanged += RefreshXP;
        XPSystem.OnLevelUp += RefreshLevel;
        CurrencySystem.OnCoinsChanged += RefreshCoins;
        FishingController.OnFishCaught += _ => RefreshActiveBait();
    }

    void OnDisable()
    {
        XPSystem.OnXPChanged -= RefreshXP;
        XPSystem.OnLevelUp -= RefreshLevel;
        CurrencySystem.OnCoinsChanged -= RefreshCoins;
        FishingController.OnFishCaught -= _ => RefreshActiveBait();
    }

    void Start()
    {
        doubleButton?.onClick.AddListener(OnDoubleClicked);
        doubleIndicator = doubleIndicator != null ? doubleIndicator : GameObject.Find("DoubleIndicator");

        if (playerNameText != null) playerNameText.text = "FISHERMAN";

        var xp = GameManager.Instance.XP;
        RefreshXP(xp.CurrentXP, xp.XPRequired(xp.Level));
        RefreshLevel(xp.Level);
        RefreshCoins(GameManager.Instance.Currency.Coins);
        RefreshActiveBait();
    }

    void Update()
    {
        bool active = fishingController != null && fishingController.IsDoubleActive;
        doubleIndicator?.SetActive(active);
        if (active && doubleTimerText != null)
            doubleTimerText.text = Mathf.CeilToInt(fishingController.DoubleTimeRemaining) + "s";
        if (doubleButton != null)
            doubleButton.interactable = !active && AdManager.Instance.CanShowAd;
    }

    // Called from UI buttons in scene
   

    void RefreshActiveBait()
    {
        if (baits == null || baits.Length == 0 || fishingController == null) return;
        int idx = fishingController.ActiveBaitIndex;
        if (idx >= baits.Length) return;
        if (activeBaitIcon != null) activeBaitIcon.sprite = baits[idx].sprite;
    }

    void OnDoubleClicked()
    {
        AdManager.Instance.ShowRewardedAd(() => fishingController.ActivateDouble(180f));
    }

    void RefreshXP(int current, int required)
    {
        if (xpSlider != null) xpSlider.value = required > 0 ? (float)current / required : 1f;
        if (xpText != null) xpText.text = $"{current} / {required}";
    }

    void RefreshLevel(int level)
    {
        if (levelText != null) levelText.text = "Lv. " + level;
    }

    void RefreshCoins(int coins)
    {
        if (coinsText != null) coinsText.text = coins.ToString("N0");
    }
}
