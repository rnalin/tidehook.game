using UnityEngine;

public class FishingCharacterAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string hookTrigger = "hook";
    [SerializeField] string hookedBool = "isHooked";

    void Reset()
    {
        animator = GetComponent<Animator>();
    }

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        FishingController.OnFishingWindowOpen += HandleHook;
        FishingController.OnFishingWindowClose += HandleReturnToIdle;
        FishingController.OnFishCaught += HandleFishCaught;
    }

    void OnDisable()
    {
        FishingController.OnFishingWindowOpen -= HandleHook;
        FishingController.OnFishingWindowClose -= HandleReturnToIdle;
        FishingController.OnFishCaught -= HandleFishCaught;
    }

    void HandleHook()
    {
        if (animator == null) return;

        animator.SetBool(hookedBool, true);
        animator.ResetTrigger(hookTrigger);
        animator.SetTrigger(hookTrigger);
    }

    void HandleFishCaught(FishData _)
    {
        HandleReturnToIdle();
    }

    void HandleReturnToIdle()
    {
        if (animator == null) return;

        animator.SetBool(hookedBool, false);
    }
}
