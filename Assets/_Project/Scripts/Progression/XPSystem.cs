using System;
using UnityEngine;

public class XPSystem : MonoBehaviour
{
    public static event Action<int, int> OnXPChanged; // (current, required)
    public static event Action<int> OnLevelUp;        // (newLevel)

    public int Level { get; private set; }
    public int CurrentXP { get; private set; }

    const int MAX_LEVEL = 10;

    public int XPRequired(int level) => Mathf.RoundToInt(100f * Mathf.Pow(level, 1.5f));

    public void Initialize(SaveData data)
    {
        Level = Mathf.Clamp(data.level, 1, MAX_LEVEL);
        CurrentXP = data.xp;
    }

    public void WriteTo(SaveData data)
    {
        data.level = Level;
        data.xp = CurrentXP;
    }

    public void AddXP(int amount)
    {
        if (Level >= MAX_LEVEL) return;

        CurrentXP += amount;
        while (Level < MAX_LEVEL && CurrentXP >= XPRequired(Level))
        {
            CurrentXP -= XPRequired(Level);
            Level++;
            OnLevelUp?.Invoke(Level);
        }

        int required = Level < MAX_LEVEL ? XPRequired(Level) : 0;
        OnXPChanged?.Invoke(CurrentXP, required);
    }
}
