using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArrowController : MonoBehaviour
{
    public float rollTorque = 20f;     // 前后滚动的力（沿本体X轴）
    public float turnTorque = 10f;     // 左右转动的力（绕Y轴）
    public float maxSpeed = 10f;       // 最大线速度，防止滚太快

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularDamping = 0.5f;
        rb.linearDamping = 0.5f;
    }

    void FixedUpdate()
    {
        float vertical = Input.GetAxis("Vertical");   // W/S
        float horizontal = Input.GetAxis("Horizontal"); // A/D

        // 1. 前后滚动（绕箭的右侧轴 = transform.right）
        if (Mathf.Abs(vertical) > 0.1f)
        {
            Vector3 torque = transform.right * -vertical * rollTorque;
            rb.AddTorque(torque);
        }

        // 2. 左右方向控制（绕世界Y轴轻微旋转）
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            Vector3 turn = Vector3.up * horizontal * turnTorque;
            rb.AddTorque(turn);
        }

        // 3. 限制最大速度（可选）
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}