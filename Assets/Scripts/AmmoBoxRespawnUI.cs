using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoBoxRespawnUI : MonoBehaviour
{
    [Header("黑幕 & UI")]
    public Image blackScreen;
    public TextMeshProUGUI boomText;

    [Header("重生设置")]
    public Transform respawnPoint_Level1;
    public Transform respawnPoint_Level2;

    public float fadeTime = 1.5f;
    public float boomDuration = 4f;
    public float pauseTime = 0.5f;

    [Header("音效")]
    public AudioClip deathSound;

    private Transform currentRespawnPoint;
    private bool isRespawning = false;

    void OnCollisionEnter(Collision collision)
    {
        if (isRespawning) return;

        string groundTag = collision.collider.tag;

        // 根据不同地面 Tag 选择重生点
        switch (groundTag)
        {
            case "Ground_Level1":
                currentRespawnPoint = respawnPoint_Level1;
                break;
            case "Ground_Level2":
                currentRespawnPoint = respawnPoint_Level2;
                break;
            default:
                Debug.LogWarning("❌ 未识别的地面标签：" + groundTag);
                return;
        }

        if (currentRespawnPoint != null)
        {
            // ✅ 强制终止炸弹倒计时（如果正在进行）
            BombTimerUI bombUI = FindObjectOfType<BombTimerUI>();
            if (bombUI != null)
            {
                bombUI.ForceCancelCountdown();
            }

            StartCoroutine(RespawnSequence());
        }
    }

    IEnumerator RespawnSequence()
    {
        isRespawning = true;

        // 播放死亡音效
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        yield return StartCoroutine(FadeToBlack(fadeTime));

        boomText.text = "YOU SACRIFICED!";
        yield return new WaitForSeconds(pauseTime);

        transform.position = currentRespawnPoint.position;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 重置士兵系统状态
        if (PlayerSwitcher.Instance != null)
        {
            PlayerSwitcher.Instance.ResetSoldier();
        }

        yield return new WaitForSeconds(boomDuration);

        boomText.text = "";
        yield return StartCoroutine(FadeToClear(fadeTime));

        isRespawning = false;
    }

    IEnumerator FadeToBlack(float duration)
    {
        float timer = 0f;
        Color c = blackScreen.color;
        while (timer < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timer / duration);
            blackScreen.color = new Color(c.r, c.g, c.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        blackScreen.color = new Color(c.r, c.g, c.b, 1f);
    }

    IEnumerator FadeToClear(float duration)
    {
        float timer = 0f;
        Color c = blackScreen.color;
        while (timer < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            blackScreen.color = new Color(c.r, c.g, c.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        blackScreen.color = new Color(c.r, c.g, c.b, 0f);
    }
}