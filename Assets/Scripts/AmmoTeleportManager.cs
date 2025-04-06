using System.Collections;
using UnityEngine;

public class AmmoTeleportManager : MonoBehaviour
{
    public static AmmoTeleportManager Instance;

    public GameObject ammoBox;
    public GameObject ammoBoxCam;
    public GameObject soldierCam;
    public GameObject soldierController;
    public GameObject ammoBoxController;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Teleport the ammo box to a target position. Optionally freeze physics to prevent weird motion from rotating platforms.
    /// </summary>
    public void TeleportAmmoBoxToTarget(Vector3 targetPosition, bool freezePhysics = false)
    {
        Vector3 safeOffset = Vector3.up * 1f;

        Rigidbody rb = ammoBox.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // ❗ 清除惯性
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (freezePhysics)
            {
                // ✅ 临时冻结，用于消除旋转惯性
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        // 🔧 关闭控制器，防止干扰
        if (ammoBoxController != null)
            ammoBoxController.SetActive(false);

        // 🚀 设置位置
        ammoBox.transform.position = targetPosition + safeOffset;
        ammoBox.transform.rotation = Quaternion.identity;

        // ✅ 开启摄像机
        if (ammoBoxCam != null)
            ammoBoxCam.SetActive(true);

        // ✅ 恢复控制器
        if (ammoBoxController != null)
            ammoBoxController.SetActive(true);

        // ✅ 延迟恢复物理状态
        if (rb != null && freezePhysics)
            StartCoroutine(ReenablePhysics(rb));

        // ❌ 关闭士兵系统（切换回弹药箱）
        if (PlayerSwitcher.Instance != null)
        {
            if (PlayerSwitcher.Instance.soldierCam != null)
                PlayerSwitcher.Instance.soldierCam.SetActive(false);

            if (PlayerSwitcher.Instance.soldierController != null)
                PlayerSwitcher.Instance.soldierController.SetActive(false);

            if (PlayerSwitcher.Instance.soldierClone != null)
                PlayerSwitcher.Instance.soldierClone.SetActive(false);
        }
    }

    private IEnumerator ReenablePhysics(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.05f);

        // ✅ 彻底解除 Freeze，恢复玩家可控（AddTorque）
        rb.constraints = RigidbodyConstraints.None;

        // ✅ 关键：脱离风扇父物体
        ammoBox.transform.SetParent(null);
    }
}