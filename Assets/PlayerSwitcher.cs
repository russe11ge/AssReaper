using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    public GameObject ammoBox;
    public GameObject soldier;

    public GameObject ammoBoxController;
    public GameObject soldierController;

    public GameObject ammoBoxCam;
    public GameObject soldierCam;

    public GameObject soldierClone;

    public float interactDistance = 3f;
    // 新增：交互提示UI，确保在Inspector中赋值，UI显示“Press E to interact”
    public GameObject interactUI;

    private bool isControllingSoldier = false;

    void Update()
    {
        if (!isControllingSoldier)
        {
            float distance = Vector3.Distance(ammoBox.transform.position, soldier.transform.position);
            if (distance <= interactDistance)
            {
                // 显示提示UI
                if (interactUI != null)
                    interactUI.SetActive(true);

                // 如果按下E键则切换
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SwitchToSoldier();
                }
            }
            else
            {
                // 离开交互距离则隐藏提示UI
                if (interactUI != null)
                    interactUI.SetActive(false);
            }
        }
    }

    void SwitchToSoldier()
    {
        isControllingSoldier = true;

        // 隐藏交互UI
        if (interactUI != null)
            interactUI.SetActive(false);

        // 切换控制权：弹药箱控制器禁用，士兵控制器启用
        ammoBoxController.SetActive(false);
        soldierController.SetActive(true);

        // 切换摄像机：弹药箱摄像机禁用，士兵摄像机启用
        ammoBoxCam.SetActive(false);
        soldierCam.SetActive(true);

        // 可选：隐藏士兵克隆对象
        soldierClone.SetActive(false);
    }

    public void SwitchBackToAmmoBox()
    {
        isControllingSoldier = false;

        soldierController.SetActive(false);
        ammoBoxController.SetActive(true);

        soldierCam.SetActive(false);
        ammoBoxCam.SetActive(true);
    }
}