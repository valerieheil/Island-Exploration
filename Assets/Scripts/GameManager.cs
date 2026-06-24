using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game controller — singleton.
/// Attach to an empty GameObject called "GameManager".
/// Wire up the uiManager and dogAnimHelper fields in the Inspector.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ── Inspector fields ──────────────────────────────────────────────────────
    [Header("Game Settings")]
    [Tooltip("How many bones the dog must find")]
    public int totalBones = 10;

    [Header("References  (assign in Inspector)")]
    public UIManager          uiManager;
    public DogAnimationHelper dogAnimHelper;

    // ── Runtime state  (read-only from outside) ───────────────────────────────
    public int  CollectedBones { get; private set; }
    public bool GameWon        { get; private set; }

    static readonly string[] Praises =
    {
        "Good boy!", "Woof! Found one!",
        "Nice sniff!", "Buried treasure!",
        "Who's a good dog?!"
    };

    // ── Unity lifecycle ───────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Auto-find if not wired in Inspector
        if (uiManager    == null) uiManager    = FindFirstObjectByType<UIManager>();
        if (dogAnimHelper == null) dogAnimHelper = FindFirstObjectByType<DogAnimationHelper>();

        uiManager?.UpdateCounter(CollectedBones, totalBones);
        uiManager?.ShowHint("WASD = move  |  SPACE = jump  |  F near a bone = dig", 5f);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Called by CollectibleBone after a successful dig.</summary>
    public void OnBoneCollected()
    {
        Debug.Log("Bone collected! Total now: " + (CollectedBones + 1));
        if (GameWon) return;

        CollectedBones++;
        uiManager?.UpdateCounter(CollectedBones, totalBones);
        uiManager?.PlayCollectFX();
        uiManager?.ShowHint(Praises[Random.Range(0, Praises.Length)], 2.5f);
        dogAnimHelper?.TriggerCelebrate();

        if (CollectedBones >= totalBones)
        {
            uiManager?.ShowAllFoundBanner(true);
            uiManager?.ShowHint("All bones found! Run home!", 6f);
        }
    }

    /// <summary>Called by GoalTrigger when the dog reaches the home/camp.</summary>
    public void OnGoalReached()
    {
        if (GameWon) return;
        int remaining = totalBones - CollectedBones;
        if (remaining > 0)
        {
            uiManager?.ShowHint(
                $"Find {remaining} more bone{(remaining == 1 ? "" : "s")} first!", 3f);
            return;
        }

        GameWon = true;
        dogAnimHelper?.TriggerCelebrate();
        uiManager?.ShowWinScreen();
        StartCoroutine(ExitGameAfterDelay(3f));
    }

    /// <summary>Wait a bit, then exit the game mode.</summary>
    private IEnumerator ExitGameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
