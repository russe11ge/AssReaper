using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform target;             // 跟随的角色
    public LayerMask collisionLayers;    // 要检测的墙面层级
    public float minDistance = 1.0f;     // 镜头最近距离
    public float maxDistance = 4.0f;     // 镜头最远距离
    public float smoothSpeed = 10.0f;    // 镜头平滑移动速度
    public float cameraRadius = 0.3f;    // 镜头球体半径

    private Vector3 currentPosition;

    void Start()
    {
        currentPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 direction = (transform.position - target.position).normalized;
        Vector3 desiredCameraPos = target.position - direction * maxDistance;

        RaycastHit hit;
        float targetDistance = maxDistance;

        // 从目标往摄像机方向做球形检测，找到最近的碰撞体
        if (Physics.SphereCast(target.position, cameraRadius, -direction, out hit, maxDistance, collisionLayers))
        {
            targetDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }

        // 计算最终摄像机位置
        Vector3 newPos = target.position - direction * targetDistance;

        // 平滑过渡
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * smoothSpeed);
    }
}