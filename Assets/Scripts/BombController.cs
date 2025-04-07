using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BombController : MonoBehaviour
{
    [Header("运动设置")]
    public float acceleration = 10f;      // 加速度
    public float maxSpeed = 8f;           // 最大速度
    public float drag = 0.98f;            // 滑动阻力
    public float controlFactor = 1f;      // 控制灵敏度

    private Rigidbody rb;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 获取WASD输入
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.W)) v = 1f;
        if (Input.GetKey(KeyCode.S)) v = -1f;
        if (Input.GetKey(KeyCode.A)) h = -1f;
        if (Input.GetKey(KeyCode.D)) h = 1f;

        // ✅ 使用主摄像机的方向
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * v + camRight * h).normalized;

        if (moveDirection.magnitude > 0)
        {
            Vector3 force = moveDirection * acceleration * controlFactor;
            rb.AddForce(force, ForceMode.Force);
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        rb.linearVelocity *= drag;
    }
}