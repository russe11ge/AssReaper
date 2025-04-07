using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    public static PlayerSwitcher Instance;

    [Header("控制对象")]
    public GameObject ammoBox;
    public GameObject soldier;

    [Header("控制器")]
    public GameObject ammoBoxController;
    public GameObject soldierController;

    [Header("摄像机")]
    public GameObject ammoBoxCam;
    public GameObject soldierCam;

    [Header("士兵替身")]
    public GameObject soldierClone;

    [Header("交互设置")]
    public float interactDistance = 3f;
    public GameObject interactUI;

    private bool isControllingSoldier = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!isControllingSoldier)
        {
            float distance = Vector3.Distance(ammoBox.transform.position, soldier.transform.position);
            if (distance <= interactDistance)
            {
                if (interactUI != null)
                    interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    SwitchToSoldier();
                }
            }
            else
            {
                if (interactUI != null)
                    interactUI.SetActive(false);
            }
        }
    }

    public void SwitchToSoldier()
    {
        isControllingSoldier = true;
        PlayerSwitcher.Instance = this;

        if (interactUI != null)
            interactUI.SetActive(false);

        // 关闭 AmmoBox 控制
        ammoBoxController.SetActive(false);
        ammoBoxCam.SetActive(false);

        // 开启士兵控制
        soldierController.SetActive(true);
        soldierCam.SetActive(true);
        soldierClone.SetActive(false);
    }

    public void SwitchBackToAmmoBox()
    {
        isControllingSoldier = false;

        // 关闭士兵控制
        soldierController.SetActive(false);
        soldierCam.SetActive(false);
        soldierClone.SetActive(false);

        // 恢复 AmmoBox 控制
        ammoBoxController.SetActive(true);
        ammoBoxCam.SetActive(true);
    }

    public void ResetSoldier()
    {
        isControllingSoldier = false;

        // 士兵重新可交互（重生时调用）
        if (soldierClone != null)
            soldierClone.SetActive(true);

        if (soldierController != null)
            soldierController.SetActive(false);

        if (soldierCam != null)
            soldierCam.SetActive(false);
    }
}