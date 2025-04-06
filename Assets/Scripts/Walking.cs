using UnityEngine;

public class Walking : MonoBehaviour
{
    public AudioClip footstepClip;
    public float moveThreshold = 0.1f;

    private AudioSource audioSource;
    private CharacterController controller;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>(); // 如果你用的是 CharacterController

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = footstepClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        // 判断是否移动
        if (controller != null && controller.velocity.magnitude > moveThreshold)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }
}