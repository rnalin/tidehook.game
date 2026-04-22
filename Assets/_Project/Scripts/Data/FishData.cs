using UnityEngine;

public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }

[CreateAssetMenu(menuName = "Tidehook/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishName;
    public Rarity rarity;
    public int xpReward;
    public int coinReward;
    public Sprite sprite;
}
