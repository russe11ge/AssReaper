using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    public GameObject ammoBox;
    public GameObject soldier;

    public GameObject ammoBoxController;
    public GameObject soldierController;

    public GameObject ammoBoxCam;
    public GameObject soldierCam;

    public float interactDistance = 3f;

    private bool isControllingSoldier = false;

    void Update()
    {
        if (!isControllingSoldier && Input.GetKeyDown(KeyCode.E))
        {
            float distance = Vector3.Distance(ammoBox.transform.position, soldier.transform.position);
            if (distance <= interactDistance)
            {
                SwitchToSoldier();
            }
        }
    }

    void SwitchToSoldier()
    {
        isControllingSoldier = true;

        // 控制切换
        ammoBoxController.SetActive(false);
        soldierController.SetActive(true);

        // 摄像机切换
        ammoBoxCam.SetActive(false);
        soldierCam.SetActive(true);
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