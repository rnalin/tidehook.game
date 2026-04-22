using System;
using System.Collections;
using UnityEngine;

public class FishingController : MonoBehaviour
{
    public static event Action OnFishingWindowOpen;
    public static event Action OnFishingWindowClose;
    public static event Action<FishData> OnFishCaught;

    [SerializeField] BaitData[] baits;

    const float BASE_CYCLE = 15f;
    const float POPUP_RATIO = 1f / 3f; // popup at 5s out of 15s

    int activeBaitIndex;
    bool isWindowActive;
    bool playerClickedThisCycle;
    bool isDoubled;
    float doubleEndTime;

    public int ActiveBaitIndex => activeBaitIndex;
    public bool IsDoubleActive => isDoubled && Time.time < doubleEndTime;
    public float DoubleTimeRemaining => IsDoubleActive ? doubleEndTime - Time.time : 0f;

    void Start() => StartCoroutine(FishingLoop());

    IEnumerator FishingLoop()
    {
        while (true)
        {
            float cycle = IsDoubleActive ? BASE_CYCLE / 2f : BASE_CYCLE;
            float triggerAt = cycle * POPUP_RATIO;

            yield return new WaitForSeconds(triggerAt);

            isWindowActive = true;
            playerClickedThisCycle = false;
            OnFishingWindowOpen?.Invoke();

            float remaining = cycle - triggerAt;
            float elapsed = 0f;
            while (elapsed < remaining && !playerClickedThisCycle)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            isWindowActive = false;
            OnFishingWindowClose?.Invoke();

            if (!playerClickedThisCycle)
                CatchFish();

            playerClickedThisCycle = false;
        }
    }

    public void PlayerClick()
    {
        if (!isWindowActive) return;
        playerClickedThisCycle = true;
        CatchFish();
    }

    void CatchFish() => OnFishCaught?.Invoke(DropTable.Pick(baits[activeBaitIndex]));

    public void SetActiveBait(int index)
    {
        if (index >= 0 && index < baits.Length)
            activeBaitIndex = index;
    }

    public void ActivateDouble(float duration)
    {
        isDoubled = true;
        doubleEndTime = Time.time + duration;
    }
}
