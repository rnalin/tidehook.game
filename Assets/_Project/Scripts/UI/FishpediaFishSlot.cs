using System;
using UnityEngine;
using UnityEngine.UI;

public class FishpediaFishSlot : MonoBehaviour
{
    [SerializeField] Image fishImage;
    [SerializeField] Button button;
    [SerializeField] GameObject selectedIndicator;

    FishData data;
    Action<FishData> onClick;

    static readonly Color Silhouette = new Color(0.18f, 0.10f, 0.05f, 1f);

    public void Setup(FishData fish, bool discovered, Action<FishData> onClickCallback)
    {
        data = fish;
        onClick = onClickCallback;

        if (fishImage != null)
        {
            fishImage.sprite = fish.sprite;
            fishImage.color = discovered ? Color.white : Silhouette;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(data));

        if (selectedIndicator != null) selectedIndicator.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectedIndicator != null) selectedIndicator.SetActive(selected);
    }
}
