using UnityEngine;

public class SwitchBomb : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    public GameObject player1Controller;
    public GameObject player2Controller;

    public GameObject player1Camera;
    public GameObject player2Camera;

    public GameObject player1FreeLook;
    public GameObject player2FreeLook;

    private bool hasSwitched = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasSwitched) return;

        if (other.CompareTag("Player")) // Player1 的 Tag 必须是 "Player"
        {
            hasSwitched = true;

            // 🔴 关闭 Player1 控制器 & Camera & FreeLook
            if (player1Controller != null) player1Controller.SetActive(false);
            if (player1Camera != null) player1Camera.SetActive(false);
            if (player1FreeLook != null) player1FreeLook.SetActive(false);

            // 🟢 启用 Player2 控制器 & Camera & FreeLook
            if (player2Controller != null) player2Controller.SetActive(true);
            if (player2Camera != null) player2Camera.SetActive(true);
            if (player2FreeLook != null) player2FreeLook.SetActive(true);

            Debug.Log("🎮 控制权已切换到 Player2");
        }
    }
}