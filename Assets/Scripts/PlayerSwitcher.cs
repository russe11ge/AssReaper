using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    public static PlayerSwitcher Instance; // 当前控制的 PlayerSwitcher

    public GameObject ammoBox;
    public GameObject soldier;

    public GameObject ammoBoxController;
    public GameObject soldierController;

    public GameObject ammoBoxCam;
    public GameObject soldierCam;

    public GameObject soldierClone;

    public float interactDistance = 3f;
    public GameObject interactUI;

    private bool isControllingSoldier = false;

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

    void SwitchToSoldier()
    {
        isControllingSoldier = true;
        PlayerSwitcher.Instance = this; // 记录当前控制的 soldier

        if (interactUI != null)
            interactUI.SetActive(false);

        ammoBoxController.SetActive(false);
        ammoBoxCam.SetActive(false);

        soldierController.SetActive(true);
        soldierCam.SetActive(true);
        soldierClone.SetActive(false);
    }

    public void SwitchBackToAmmoBox()
    {
        isControllingSoldier = false;

        soldierController.SetActive(false);
        soldierCam.SetActive(false);
        soldierClone.SetActive(false);

        ammoBoxController.SetActive(true);
        ammoBoxCam.SetActive(true);
    }
}