using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BombTimerUI : MonoBehaviour
{
    public float countdownTime = 5f;
    public Transform respawnPoint;
    public TextMeshProUGUI countdownText;
    public Image blackScreen;  // 拖入 BlackScreen Image

    private Coroutine countdownRoutine;
    private bool isCounting = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TriggerZone") && !isCounting)
        {
            countdownRoutine = StartCoroutine(StartCountdown());
        }

        if (other.CompareTag("CancelZone") && isCounting)
        {
            StopCoroutine(countdownRoutine);
            countdownText.text = "";
            isCounting = false;
            Debug.Log("✅ 解除成功，倒计时取消");
        }
    }

    IEnumerator StartCountdown()
    {
        isCounting = true;
        float timer = countdownTime;

        while (timer > 0f)
        {
            countdownText.text = "You'll explode in:" + Mathf.Ceil(timer).ToString() + " sec";
            timer -= Time.deltaTime;
            yield return null;
        }

        // 显示 BOOM 文字
        countdownText.text = "BOOOOOOOM! YOU SACRIFICED!";

        // 开始渐变黑幕
        yield return StartCoroutine(FadeToBlack(3f)); // 3秒变黑

        // 等待黑屏1秒
        yield return new WaitForSeconds(0.3f);

        // 重生
        Respawn();

        // 保持 BOOM 显示 4 秒
        yield return new WaitForSeconds(1f);

        // 清除文字
        countdownText.text = "";

        // 还原黑幕透明
        yield return StartCoroutine(FadeToClear(1f)); // 1秒淡出黑幕
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

        blackScreen.color = new Color(c.r, c.g, c.b, 1f); // 全黑
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

        blackScreen.color = new Color(c.r, c.g, c.b, 0f); // 全透明
    }

    void Respawn()
    {
        transform.position = respawnPoint.position;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        isCounting = false;
    }
}