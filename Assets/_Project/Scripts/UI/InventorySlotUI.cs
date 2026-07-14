using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI countText;

    public void SetFish(FishData fish, int count)
    {
        gameObject.SetActive(true);
        if (icon != null) { icon.sprite = fish.sprite; icon.color = Color.white; }
        if (countText != null) countText.text = count.ToString();
    }

    public void SetEmpty()
    {
        if (icon != null) { icon.sprite = null; icon.color = new Color(0, 0, 0, 0); }
        if (countText != null) countText.text = "";
    }
}
