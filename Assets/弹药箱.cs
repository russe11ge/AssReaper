using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class 弹药箱 : MonoBehaviour
{
    public float rollTorque = 120f;
    public float maxSpeed = 10f;
    public Transform cameraTransform; // 🎥 引用 Cinemachine Camera 的 transform

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularDamping = 0.1f;
        rb.linearDamping = 0.1f;

        // 如果没手动赋值，自动找 Main Camera
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 没输入就退出
        if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f)
            return;

        // 👉 获取摄像机的前方和右方（忽略Y方向）
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // ✅ 计算“相机朝向”的移动方向
        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // 使用世界空间扭力（让物体滚动）
        Vector3 torqueDir = Vector3.Cross(Vector3.up, moveDir);
        rb.AddTorque(torqueDir * rollTorque, ForceMode.Force);

        // 限速
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}