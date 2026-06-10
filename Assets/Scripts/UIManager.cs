using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles all on-screen UI: bone counter, hint text, collect flash,
/// win screen, and the "all bones found" banner.
///
/// Attach to the same GameObject as GameManager, or a separate one.
/// Wire every public field in the Inspector.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class UIManager : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────────────
    [Header("HUD")]
    [Tooltip("TextMeshProUGUI showing  'Bones: 3 / 10'")]
    public TextMeshProUGUI counterText;

    [Tooltip("Centre-screen TextMeshProUGUI for hints and praise messages")]
    public TextMeshProUGUI hintText;

    [Header("Screens & Banners")]
    [Tooltip("Panel shown when all bones are collected (enable on win)")]
    public GameObject winScreen;

    [Tooltip("Small banner: 'All bones found! Run home!'")]
    public GameObject allFoundBanner;

    [Header("Collect Flash")]
    [Tooltip("Full-screen Image — set Color alpha to 0 in Inspector")]
    public Image collectFlashImage;

    [Header("Audio")]
    public AudioClip collectSFX;   // short bark / chimes
    public AudioSource winSFX;       // victory howl
    public AudioSource ambientLoop;  // ocean / wind (looping)

    // ── Private ───────────────────────────────────────────────────────────────
    Coroutine hintRoutine;
    [Tooltip("Optional AudioSource for playing UI SFX; assign in Inspector")]
    public AudioSource audioSource;

    // ── Unity lifecycle ───────────────────────────────────────────────────────
    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        // Ensure the UI AudioSource won't play automatically if it has a clip
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.clip = null;
        if (ambientLoop != null && !ambientLoop.isPlaying) ambientLoop.Play();
        if (winScreen)      winScreen.SetActive(false);
        if (allFoundBanner) allFoundBanner.SetActive(false);
        if (hintText)       hintText.text = "";
        if (collectFlashImage) collectFlashImage.color = new Color(1, 1, 1, 0);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void UpdateCounter(int current, int total)
    {
        if (counterText) counterText.text = $"Bones: {current} / {total}";
    }

    /// <summary>
    /// Show a hint for <paramref name="duration"/> seconds.
    /// Pass duration &lt;= 0 to show indefinitely until ShowHint("") is called.
    /// </summary>
    public void ShowHint(string message, float duration = 3f)
    {
        if (hintText == null) return;
        if (hintRoutine != null) StopCoroutine(hintRoutine);

        hintText.text = message;

        if (duration > 0f)
            hintRoutine = StartCoroutine(ClearHintAfter(duration));
    }

    IEnumerator ClearHintAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (hintText) hintText.text = "";
    }

    public void PlayCollectFX()
    {
        if (collectSFX != null)
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            if (audioSource != null)
                audioSource.PlayOneShot(collectSFX);
        }
        if (collectFlashImage) StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        float t = 0f, dur = 0.4f;
        while (t < dur)
        {
            t += Time.deltaTime;
            collectFlashImage.color = new Color(1f, 0.85f, 0.2f,
                Mathf.Lerp(0.45f, 0f, t / dur));
            yield return null;
        }
        collectFlashImage.color = new Color(1, 1, 1, 0);
    }

    public void ShowAllFoundBanner(bool show)
    {
        //if (allFoundBanner) allFoundBanner.SetActive(show);
    }

    public void ShowWinScreen()
    {
        winSFX?.Play();
        if (winScreen) winScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }
}
