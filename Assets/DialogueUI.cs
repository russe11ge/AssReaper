using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public AudioSource audioSource;
    public AudioClip dialogueSFX;

    public float typingSpeed = 0.03f;

    private string[] lines;
    private int index = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public static DialogueUI Instance;

    void Awake()
    {
        Instance = this;
        dialogueBox.SetActive(false);
    }

    void Update()
    {
        if (dialogueBox.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = lines[index];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void ShowDialogue(string[] newLines)
    {
        if (newLines == null || newLines.Length == 0) return;

        lines = newLines;
        index = 0;
        dialogueBox.SetActive(true);

        // ✅ 进入对话时播放一次音效
        if (dialogueSFX != null && audioSource != null)
        {
            audioSource.Stop();      // 保证重新播放
            audioSource.clip = dialogueSFX;
            audioSource.Play();
        }

        typingCoroutine = StartCoroutine(TypeLine(lines[index]));
    }

    void NextLine()
    {
        index++;

        if (dialogueSFX != null && audioSource != null)
        {
            audioSource.Stop();              // 🔁 重新播放长音效
            audioSource.clip = dialogueSFX;
            audioSource.Play();
        }

        if (index < lines.Length)
        {
            typingCoroutine = StartCoroutine(TypeLine(lines[index]));
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
    }

    public bool IsDialogueActive()
    {
        return dialogueBox.activeSelf;
    }

    public void ForceEndDialogue()
    {
        StopAllCoroutines();                 // 停止打字机效果
        dialogueText.text = "";
        dialogueBox.SetActive(false);
        isTyping = false;

        // ✅ 停止音效
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }
}