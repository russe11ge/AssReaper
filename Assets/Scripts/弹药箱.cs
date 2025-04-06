using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class 弹药箱 : MonoBehaviour
{
    public float rollTorque = 120f;
    public float maxSpeed = 10f;
    public Transform cameraTransform;

    public AudioClip moveSound;
    public float moveThreshold = 0.1f;

    private Rigidbody rb;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularDamping = 0.1f;
        rb.linearDamping = 0.1f;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = moveSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f)
        {
            if (audioSource.isPlaying)
                audioSource.Pause();

            return;
        }

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        Vector3 torqueDir = Vector3.Cross(Vector3.up, moveDir);
        rb.AddTorque(torqueDir * rollTorque, ForceMode.Force);

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        if (rb.linearVelocity.magnitude > moveThreshold)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
    }
}