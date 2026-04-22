using System;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    const int MAX_ADS_PER_SESSION = 3;

    int sessionAdCount;

    public bool CanShowAd => sessionAdCount < MAX_ADS_PER_SESSION;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        // TODO Fase 2: MobileAds.Initialize(_ => LoadRewardedAd());
    }

    public void ShowRewardedAd(Action onRewarded)
    {
        if (!CanShowAd) return;

#if UNITY_EDITOR
        sessionAdCount++;
        onRewarded?.Invoke();
#else
        // TODO Fase 2: substituir por rewardedAd.Show(_ => { sessionAdCount++; onRewarded?.Invoke(); });
        sessionAdCount++;
        onRewarded?.Invoke();
#endif
    }
}
