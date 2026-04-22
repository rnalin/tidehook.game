using UnityEngine;
using UnityEngine.UI;

public class FishingPopup : MonoBehaviour
{
    [SerializeField] GameObject popupRoot;
    [SerializeField] Button catchButton;
    [SerializeField] FishingController fishingController;

    void OnEnable()
    {
        FishingController.OnFishingWindowOpen += Show;
        FishingController.OnFishingWindowClose += Hide;
    }

    void OnDisable()
    {
        FishingController.OnFishingWindowOpen -= Show;
        FishingController.OnFishingWindowClose -= Hide;
    }

    void Start()
    {
        popupRoot.SetActive(false);
        catchButton.onClick.AddListener(fishingController.PlayerClick);
    }

    void Show() => popupRoot.SetActive(true);
    void Hide() => popupRoot.SetActive(false);
}
