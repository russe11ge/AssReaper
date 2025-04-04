using UnityEngine;

public class TrainEndTrigger : MonoBehaviour
{
    public Transform teleportTarget;
    public GameObject ammoBox;
    public TrainSwitcher trainSwitcher; // 👈 新增：火车控制引用

    private bool hasTeleported = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTeleported) return;

        if (other.CompareTag("Train") || other.GetComponent<TrainSwitcher>() != null)
        {
            if (ammoBox != null && teleportTarget != null)
            {
                ammoBox.transform.position = teleportTarget.position + Vector3.up;
                hasTeleported = true;
                Debug.Log("✅ 玩家已被火车传送到终点");

                // ✅ 切换视角
                if (trainSwitcher != null)
                {
                    trainSwitcher.SwitchBackToPlayer();
                }
                else
                {
                    Debug.LogWarning("⚠️ TrainSwitcher 没有被赋值！");
                }
            }
        }
    }
}