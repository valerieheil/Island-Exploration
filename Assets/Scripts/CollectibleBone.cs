using UnityEngine;
using System.Collections;

/// <summary>
/// Attach to any bone/collectible in the scene.
///
/// Workflow:
///   1. Dog walks into the trigger zone.
///   2. "Press F to dig!" hint appears on screen.
///   3. Player presses F — existing dog controller plays dig animation,
///      this script waits for digDuration then collects the bone.
///   4. GameManager.OnBoneCollected() is called → celebrate animation plays.
///
/// Setup:
///   • Give this GameObject a Collider with Is Trigger = true.
///   • Tag the dog/player GameObject as "Player".
///   • Optionally assign a particle effect to collectParticle.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CollectibleBone : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────────────
    [Header("Interaction")]
    [Tooltip("Key to press when near a bone (should match dig key in dog controller)")]
    public KeyCode digKey = KeyCode.F;

    [Tooltip("Seconds to wait after pressing dig key before collecting " +
             "(should match dog dig animation length)")]
    public float digDuration = 1.5f;

    [Header("Visuals")]
    [Tooltip("Optional particle played on collect — will be detached and destroyed")]
    public ParticleSystem collectParticle;

    [Tooltip("Optional marker mesh (bone sticking out of ground, mound, etc.) " +
             "that bobs to attract attention")]
    public Transform bobTarget;
    public float bobAmount = 0.12f;
    public float bobSpeed  = 2.2f;

    // ── Private ───────────────────────────────────────────────────────────────
    bool        isNearby  = false;
    bool        isBeingDug = false;
    UIManager   ui;
    Vector3     bobOrigin;

    // ── Unity lifecycle ───────────────────────────────────────────────────────
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        ui = FindFirstObjectByType<UIManager>();
        if (bobTarget) bobOrigin = bobTarget.localPosition;
    }

    void Update()
    {
        // Bob animation draws attention in the world
        if (bobTarget && !isBeingDug)
        {
            float y = bobOrigin.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            bobTarget.localPosition = new Vector3(bobOrigin.x, y, bobOrigin.z);
        }

        // Dig input — only when dog is inside the trigger
        if (isNearby && !isBeingDug && Input.GetKeyDown(digKey))
            StartCoroutine(DigSequence());
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isNearby = true;
        // Persistent hint (no duration) — cleared in OnTriggerExit or when collected
        ui?.ShowHint("Press F to dig!", -1f);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isNearby = false;
        ui?.ShowHint("", 0f);   // clear immediately
    }

    // ── Dig sequence ──────────────────────────────────────────────────────────
    IEnumerator DigSequence()
    {
        isBeingDug = true;
        isNearby   = false;

        // Show digging feedback
        ui?.ShowHint("Digging...", digDuration + 0.5f);

        // Wait for dig animation
        yield return new WaitForSeconds(digDuration);

        // Spawn and detach collect particle so it survives GameObject destruction
        if (collectParticle != null)
        {
            collectParticle.transform.SetParent(null);
            collectParticle.Play();
            Destroy(collectParticle.gameObject,
                    collectParticle.main.duration + collectParticle.main.startLifetime.constantMax + 0.5f);
        }

        // Notify game manager (this also triggers celebrate animation on dog)
        ui?.PlayCollectFX();
        GameManager.Instance?.OnBoneCollected();

        Destroy(gameObject);
    }
}
