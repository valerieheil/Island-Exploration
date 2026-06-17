using System.Collections;
using UnityEngine;

public class DigBone : MonoBehaviour
{
    [Header("References")]
    public GameObject bone;
    public float digTime = 2f;

    private bool playerNearby = false;
    private bool digging = false;

    private void Update()
    {
        if (playerNearby && !digging && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Dig());
        }
    }

    private IEnumerator Dig()
    {
        digging = true;

        // kurze Grabzeit
        yield return new WaitForSeconds(digTime);

        // Knochen sichtbar machen
        bone.SetActive(true);

        // Sandhügel verschwinden lassen
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
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