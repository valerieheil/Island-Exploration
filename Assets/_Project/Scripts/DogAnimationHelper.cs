using UnityEngine;

/// <summary>
/// Safe bridge between game logic and the dog's Animator.
///
/// Setup:
///   1. Attach to the same GameObject as the dog's Animator
///      (or to GameManager and drag the dog in).
///   2. Open the dog's Animator Controller — note the exact names
///      of the Celebrate and Dig triggers/bools.
///   3. Set those names in the Inspector fields below.
///
/// The script checks whether each parameter actually exists before
/// calling SetTrigger/SetBool so it won't spam errors if a name is wrong.
/// </summary>
public class DogAnimationHelper : MonoBehaviour
{
    [Header("Dog Reference")]
    [Tooltip("The Animator on the dog. Leave empty to auto-find.")]
    public Animator dogAnimator;

    [Header("Animator Parameter Names")]
    [Tooltip("Name of the Trigger parameter for the celebrate / happy animation")]
    public string celebrateTrigger = "Celebrate";

    [Tooltip("Name of the Trigger parameter for the dig animation " +
             "(only needed if you want to trigger it via code, " +
             "not required if the dog controller already handles it)")]
    public string digTrigger = "Dig";

    // ── Unity lifecycle ───────────────────────────────────────────────────────
    void Start()
    {
        if (dogAnimator == null)
        {
            // Try to find animator on this object first, then anywhere in scene
            dogAnimator = GetComponentInChildren<Animator>();
            if (dogAnimator == null)
                dogAnimator = FindFirstObjectByType<Animator>();

            if (dogAnimator != null)
                Debug.Log($"[DogAnimationHelper] Auto-found Animator on '{dogAnimator.gameObject.name}'");
            else
                Debug.LogWarning("[DogAnimationHelper] No Animator found. " +
                                 "Drag the dog into the dogAnimator field in the Inspector.");
        }
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void TriggerCelebrate() => SafeTrigger(celebrateTrigger);
    public void TriggerDig()       => SafeTrigger(digTrigger);

    // ── Helpers ───────────────────────────────────────────────────────────────
    void SafeTrigger(string paramName)
    {
        if (dogAnimator == null || string.IsNullOrEmpty(paramName)) return;

        foreach (var p in dogAnimator.parameters)
        {
            if (p.name == paramName && p.type == AnimatorControllerParameterType.Trigger)
            {
                dogAnimator.SetTrigger(paramName);
                return;
            }
        }

        // Not found — give a helpful message
        Debug.LogWarning($"[DogAnimationHelper] Trigger '{paramName}' not found in " +
                         $"'{dogAnimator.runtimeAnimatorController?.name}'. " +
                         "Check the 'Celebrate Trigger' field in the Inspector.");
    }
}
