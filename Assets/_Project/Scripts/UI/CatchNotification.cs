using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatchNotification : MonoBehaviour
{
    [SerializeField] GameObject notificationRoot;
    [SerializeField] Image fishIcon;
    [SerializeField] TextMeshProUGUI rewardText;

    void OnEnable() => FishingController.OnFishCaught += Show;
    void OnDisable() => FishingController.OnFishCaught -= Show;

    void Awake() => notificationRoot.SetActive(false);

    void Show(FishData fish)
    {
        if (fishIcon != null) fishIcon.sprite = fish.sprite;
        if (rewardText != null) rewardText.text = $"+{fish.coinReward}  +{fish.xpReward}xp";
        StopAllCoroutines();
        StartCoroutine(ShowThenHide());
    }

    IEnumerator ShowThenHide()
    {
        notificationRoot.SetActive(true);
        yield return new WaitForSeconds(2f);
        notificationRoot.SetActive(false);
    }
}
