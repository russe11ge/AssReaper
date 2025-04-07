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

    [Header("火车音效")]
    public AudioSource trainAudioSource;
    public AudioClip trainSound;

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

        // ✅ 播放火车动画
        if (trainAnimation != null && !trainAnimation.isPlaying)
            trainAnimation.Play();

        // ✅ 播放火车音效
        if (trainAudioSource != null && trainSound != null)
        {
            trainAudioSource.clip = trainSound;
            trainAudioSource.Play();
        }

        Debug.Log("🚂 已切换到火车模式并播放动画与音效");
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