using UnityEngine;

public class DialogueTarget : MonoBehaviour
{
    public string[] dialogueLines;
    public GameObject interactPromptUI;

    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !DialogueUI.Instance.IsDialogueActive())
        {
            DialogueUI.Instance.ShowDialogue(dialogueLines);
            if (interactPromptUI) interactPromptUI.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactPromptUI) interactPromptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactPromptUI) interactPromptUI.SetActive(false);

            // ✅ 离开时强制结束对话
            if (DialogueUI.Instance.IsDialogueActive())
            {
                DialogueUI.Instance.ForceEndDialogue();
            }
        }
    }
}