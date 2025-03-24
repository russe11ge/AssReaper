using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class 弹药箱 : MonoBehaviour
{
    public float rollTorque = 120f;
    public float maxSpeed = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularDamping = 0.1f;
        rb.linearDamping = 0.1f;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D
        float v = Input.GetAxisRaw("Vertical");   // W/S

        // 世界坐标方向控制：始终按世界前/右方向滚动，不依赖当前自身方向
        Vector3 inputDir = new Vector3(h, 0f, v);

        if (inputDir.sqrMagnitude > 0.01f)
        {
            // 计算世界空间下的扭力方向
            Vector3 torqueAxis = Vector3.Cross(Vector3.up, inputDir.normalized);

            // 加力：世界空间方向扭力，适应任何朝向
            rb.AddTorque(torqueAxis * rollTorque, ForceMode.Force);
        }

        // 限速
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}
