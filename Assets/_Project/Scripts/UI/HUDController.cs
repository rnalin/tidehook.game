using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] Slider xpSlider;
    [SerializeField] GameObject doubleIndicator;
    [SerializeField] TextMeshProUGUI doubleTimerText;
    [SerializeField] Button doubleButton;
    [SerializeField] FishingController fishingController;

    void OnEnable()
    {
        XPSystem.OnXPChanged += RefreshXP;
        XPSystem.OnLevelUp += RefreshLevel;
        CurrencySystem.OnCoinsChanged += RefreshCoins;
    }

    void OnDisable()
    {
        XPSystem.OnXPChanged -= RefreshXP;
        XPSystem.OnLevelUp -= RefreshLevel;
        CurrencySystem.OnCoinsChanged -= RefreshCoins;
    }

    void Start()
    {
        doubleButton?.onClick.AddListener(OnDoubleClicked);

        var xp = GameManager.Instance.XP;
        RefreshXP(xp.CurrentXP, xp.XPRequired(xp.Level));
        RefreshLevel(xp.Level);
        RefreshCoins(GameManager.Instance.Currency.Coins);
    }

    void Update()
    {
        bool active = fishingController.IsDoubleActive;
        doubleIndicator?.SetActive(active);

        if (active && doubleTimerText != null)
            doubleTimerText.text = Mathf.CeilToInt(fishingController.DoubleTimeRemaining) + "s";

        if (doubleButton != null)
            doubleButton.interactable = !active && AdManager.Instance.CanShowAd;
    }

    void OnDoubleClicked()
    {
        AdManager.Instance.ShowRewardedAd(() => fishingController.ActivateDouble(180f));
    }

    void RefreshXP(int current, int required)
    {
        if (xpSlider == null) return;
        xpSlider.value = required > 0 ? (float)current / required : 1f;
    }

    void RefreshLevel(int level)
    {
        if (levelText != null) levelText.text = "Lv " + level;
    }

    void RefreshCoins(int coins)
    {
        if (coinsText != null) coinsText.text = coins.ToString("N0");
    }
}
