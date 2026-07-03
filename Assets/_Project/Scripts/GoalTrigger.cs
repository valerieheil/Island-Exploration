using UnityEngine;

/// <summary>
/// Place at the final goal (home / camp / boat).
/// The dog must have all bones before this triggers a win.
///
/// Setup:
///   • Add a Collider to this GameObject and tick Is Trigger.
///   • Optionally assign a beacon light or particle to goalBeacon.
///   • Tag the dog as "Player".
/// </summary>
[RequireComponent(typeof(Collider))]
public class GoalTrigger : MonoBehaviour
{
    [Header("Visual Beacon")]
    [Tooltip("Optional: a light / particle system acting as a beacon above the goal")]
    public GameObject goalBeacon;

    [Header("Hint shown when dog arrives but has not collected all bones")]
    public string notReadyHint = "Find all the bones first!";

    // ── Unity lifecycle ───────────────────────────────────────────────────────
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        if (goalBeacon) goalBeacon.SetActive(true);
    }

    // ── Trigger events ────────────────────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null &&
            GameManager.Instance.CollectedBones < GameManager.Instance.totalBones)
        {
            FindFirstObjectByType<UIManager>()?.ShowHint(notReadyHint, 3f);
        }

        GameManager.Instance?.OnGoalReached();
    }
}
