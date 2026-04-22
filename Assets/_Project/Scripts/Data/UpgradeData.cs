using UnityEngine;

[CreateAssetMenu(menuName = "Tidehook/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public int tier;
    public int cost;
    public int requiredLevel;
    public string description;
    public Sprite previewSprite;
}
