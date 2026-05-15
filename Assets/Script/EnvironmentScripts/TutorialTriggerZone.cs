using UnityEngine;
using TMPro;

public class TutorialTriggerZone : MonoBehaviour
{
    [Header("Tutorial")]
    public TutorialInteractable tutorialInteractable;

    [Header("UI")]
    public TMP_Text interactText;
    public string message = "Press E to Open Tutorial";

    private bool playerInside = false;

    void Start()
    {
        if (tutorialInteractable == null)
            tutorialInteractable = GetComponentInParent<TutorialInteractable>();

        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            if (tutorialInteractable != null)
                tutorialInteractable.Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        if (interactText != null)
        {
            interactText.text = message;
            interactText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }
}