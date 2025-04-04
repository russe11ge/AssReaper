using UnityEngine;

public class TrainSwitcher : MonoBehaviour
{
    public GameObject ammoBox;
    public GameObject ammoBoxCam;
    public GameObject ammoBoxController;

    public GameObject trainCam;
    public Animation trainAnimation;

    public float interactDistance = 3f;
    public GameObject interactUI;

    private bool hasSwitched = false;

    void Update()
    {
        if (hasSwitched) return;

        float distance = Vector3.Distance(ammoBox.transform.position, transform.position);

        if (distance <= interactDistance)
        {
            if (interactUI != null)
                interactUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                RideTrain();
            }
        }
        else
        {
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    void RideTrain()
    {
        hasSwitched = true;

        if (interactUI != null)
            interactUI.SetActive(false);

        if (ammoBoxController != null)
            ammoBoxController.SetActive(false);

        if (ammoBoxCam != null)
            ammoBoxCam.SetActive(false);

        if (trainCam != null)
            trainCam.SetActive(true);

        if (trainAnimation != null && !trainAnimation.isPlaying)
            trainAnimation.Play();
    }

    public void SwitchBackToPlayer()
    {
        if (trainCam != null)
            trainCam.SetActive(false);

        if (ammoBoxCam != null)
            ammoBoxCam.SetActive(true);

        if (ammoBoxController != null)
            ammoBoxController.SetActive(true);

        Debug.Log("🎮 已切换回玩家控制");
    }
}