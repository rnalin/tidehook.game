using System;
using UnityEngine;

public class CurrencySystem : MonoBehaviour
{
    public static event Action<int> OnCoinsChanged;

    public int Coins { get; private set; }

    public void Initialize(SaveData data) => Coins = data.coins;

    public void WriteTo(SaveData data) => data.coins = Coins;

    public void AddCoins(int amount)
    {
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins);
    }

    public bool SpendCoins(int amount)
    {
        if (Coins < amount) return false;
        Coins -= amount;
        OnCoinsChanged?.Invoke(Coins);
        return true;
    }
}
