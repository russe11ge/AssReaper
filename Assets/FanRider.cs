using UnityEngine;

public class FanRider : MonoBehaviour
{
    public string fanTag = "Fan";  // 给风扇物体打上这个Tag
    private Transform fanTransform;
    private Vector3 localOffset;
    private bool isOnFan = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(fanTag))
        {
            fanTransform = collision.collider.transform;
            localOffset = fanTransform.InverseTransformPoint(transform.position);
            isOnFan = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(fanTag))
        {
            isOnFan = false;
            fanTransform = null;
        }
    }

    void LateUpdate()
    {
        if (isOnFan && fanTransform != null)
        {
            Vector3 newWorldPos = fanTransform.TransformPoint(localOffset);
            Vector3 delta = newWorldPos - transform.position;

            // 强制“粘”在风扇上
            rb.MovePosition(rb.position + delta);

            // 同步旋转
            rb.MoveRotation(fanTransform.rotation);
        }
    }
}