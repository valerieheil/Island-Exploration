using UnityEngine;

/// <summary>
/// "Dog sniffing" guide — a Particle System that drifts its particles
/// toward the nearest uncollected bone (or toward the goal when all are found).
///
/// Setup:
///   1. Add a Particle System as a child of the dog (near the nose).
///   2. Attach this script to that Particle System.
///   3. Tune particle size/colour to look like small scent wisps.
///   4. The particles automatically point toward the nearest target.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class DogScentTrail : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Recalculate direction every N seconds (lower = more responsive)")]
    public float updateInterval = 0.35f;

    [Tooltip("Only show scent particles when a bone is closer than this")]
    public float maxRange = 100f;

    [Tooltip("Scent particle speed (units/sec)")]
    public float scentSpeed = 2.5f;

    [Tooltip("Slight upward drift so particles float like real scent")]
    public float upDrift = 0.4f;

    // ── Private ───────────────────────────────────────────────────────────────
    ParticleSystem                            ps;
    ParticleSystem.VelocityOverLifetimeModule vel;
    float                                     timer;

    // ── Unity lifecycle ───────────────────────────────────────────────────────
    void Start()
    {
        ps  = GetComponent<ParticleSystem>();
        vel = ps.velocityOverLifetime;
        vel.enabled = true;

        // Make sure velocity space is World so direction matches world axes
        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ps.Stop();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < updateInterval) return;
        timer = 0f;

        Transform target = FindTarget();

        if (target == null)
        {
            TryStop();
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist > maxRange) { TryStop(); return; }

        // Build world-space velocity toward target
        Vector3 dir = (target.position - transform.position).normalized;
        vel.x = new ParticleSystem.MinMaxCurve(dir.x * scentSpeed);
        vel.y = new ParticleSystem.MinMaxCurve(dir.y * scentSpeed + upDrift);
        vel.z = new ParticleSystem.MinMaxCurve(dir.z * scentSpeed);

        if (!ps.isPlaying) ps.Play();
    }

    void TryStop() { if (ps.isPlaying) ps.Stop(); }

    Transform FindTarget()
    {
        if (GameManager.Instance == null) return null;

        // All bones collected → point to goal
        if (GameManager.Instance.CollectedBones >= GameManager.Instance.totalBones)
        {
            var goal = FindFirstObjectByType<GoalTrigger>();
            return goal != null ? goal.transform : null;
        }

        // Nearest uncollected bone
        CollectibleBone[] bones =
            FindObjectsByType<CollectibleBone>(FindObjectsSortMode.None);

        Transform nearest = null;
        float     best    = float.MaxValue;
        foreach (var b in bones)
        {
            float d = Vector3.Distance(transform.position, b.transform.position);
            if (d < best) { best = d; nearest = b.transform; }
        }
        return nearest;
    }
}
