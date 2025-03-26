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

    public void TeleportAmmoBoxToTarget(Vector3 targetPosition)
    {
        // 传送弹药箱
        Vector3 safeOffset = Vector3.up * 1f; // 你可以根据弹药箱大小调整这个偏移
        ammoBox.transform.position = targetPosition + safeOffset;

        // 切换相机和控制权
        soldierCam.SetActive(false);
        ammoBoxCam.SetActive(true);

        soldierController.SetActive(false);
        ammoBoxController.SetActive(true);
    }
}