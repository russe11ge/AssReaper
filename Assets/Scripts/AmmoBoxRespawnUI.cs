using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoBoxRespawnUI : MonoBehaviour
{
    [Header("黑幕 & UI")]
    public Image blackScreen;             // 拖入黑幕 UI Image
    public TextMeshProUGUI boomText;      // 拖入显示 BOOM 的文本

    [Header("重生设置")]
    public Transform respawnPoint;        // 拖入重生位置
    public float fadeTime = 1.5f;         // 黑幕渐变时间
    public float boomDuration = 4f;       // BOOM 显示时间
    public float pauseTime = 0.5f;        // 黑幕全黑时等待时间

    private bool isRespawning = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!isRespawning && collision.collider.CompareTag("Ground"))
        {
            StartCoroutine(RespawnSequence());
        }
    }

    IEnumerator RespawnSequence()
    {
        isRespawning = true;

        // 开始黑屏
        yield return StartCoroutine(FadeToBlack(fadeTime));

        // 显示 BOOM
        boomText.text = "YOU SACRIFICED!";

        // 暂停一会（黑屏停留）
        yield return new WaitForSeconds(pauseTime);

        // 执行重生
        transform.position = respawnPoint.position;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 保持 BOOM 显示
        yield return new WaitForSeconds(boomDuration);

        // 清除 BOOM + 淡出黑幕
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