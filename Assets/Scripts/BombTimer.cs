using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BombTimerUI : MonoBehaviour
{
    public float countdownTime = 5f;
    public Transform respawnPoint;
    public TextMeshProUGUI countdownText;
    public Image blackScreen;

    private Coroutine countdownRoutine;
    private bool isCounting = false;

    void OnTriggerEnter(Collider other)
    {
        // 启动倒计时
        if (other.CompareTag("TriggerZone") && !isCounting)
        {
            countdownRoutine = StartCoroutine(StartCountdown());
        }

        // 取消倒计时（CancelZone 或 Ground）
        if ((other.CompareTag("CancelZone") || other.CompareTag("Ground_Level1")) && isCounting)
        {
            ForceCancelCountdown();
        }
    }

    public void ForceCancelCountdown()
    {
        if (isCounting && countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownText.text = "";
            isCounting = false;
            Debug.Log("❌ 倒计时因牺牲被强制终止");
        }
    }

    IEnumerator StartCountdown()
    {
        isCounting = true;
        float timer = countdownTime;

        while (timer > 0f)
        {
            countdownText.text = "You'll explode in: " + Mathf.Ceil(timer).ToString() + " sec";
            timer -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "BOOOOOOOM! YOU SACRIFICED!";
        yield return StartCoroutine(FadeToBlack(3f));
        yield return new WaitForSeconds(0.3f);

        Respawn();
        yield return new WaitForSeconds(1f);

        countdownText.text = "";
        yield return StartCoroutine(FadeToClear(1f));
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