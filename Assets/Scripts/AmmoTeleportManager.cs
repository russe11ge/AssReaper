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
        Vector3 safeOffset = Vector3.up * 1f;
        ammoBox.transform.position = targetPosition + safeOffset;

        // 关闭当前控制的士兵系统（使用静态 PlayerSwitcher.Instance）
        if (PlayerSwitcher.Instance != null)
        {
            if (PlayerSwitcher.Instance.soldierCam != null)
                PlayerSwitcher.Instance.soldierCam.SetActive(false);

            if (PlayerSwitcher.Instance.soldierController != null)
                PlayerSwitcher.Instance.soldierController.SetActive(false);

            if (PlayerSwitcher.Instance.soldierClone != null)
                PlayerSwitcher.Instance.soldierClone.SetActive(false);
        }

        ammoBoxCam.SetActive(true);
        ammoBoxController.SetActive(true);
    }
}