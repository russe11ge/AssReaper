using UnityEngine;

public class DropSound : MonoBehaviour
{
    public AudioClip soundEffect;
    private AudioSource audioSource;

    private GameObject currentTarget = null;
    private bool isInsideTarget = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            // 如果还没有进入任何 target
            if (!isInsideTarget)
            {
                currentTarget = other.gameObject;
                isInsideTarget = true;
                audioSource.PlayOneShot(soundEffect);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target") && other.gameObject == currentTarget)
        {
            // 离开当前板子，允许下次进入时重新播放
            isInsideTarget = false;
            currentTarget = null;
        }
    }
}