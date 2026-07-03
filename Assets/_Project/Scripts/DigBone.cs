using System.Collections;
using UnityEngine;

public class DigBone : MonoBehaviour
{
    [Header("References")]
    public GameObject bone;
    public GameObject sandPile; 
    public float digTime = 2f;

    private bool playerNearby = false;
    private bool digging = false;

    UIManager   ui;

    void Start()
    {
        ui = FindFirstObjectByType<UIManager>();   
    }
    private void Update()
    {
        if (playerNearby && !digging && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Dig());
        }
    }

    private IEnumerator Dig()
    {
        Debug.Log("Digging started.");
        digging = true;

        // kurze Grabzeit
        yield return new WaitForSeconds(digTime);

        // Knochen sichtbar machen
        bone.SetActive(true);

        // Sandhügel verschwinden lassen
        sandPile.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            ui?.ShowHint("Press F to dig!", -1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}