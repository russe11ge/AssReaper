using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BombTimerUI : MonoBehaviour
{
    [Header("倒计时设置")]
    public float countdownTime = 5f;
    public Transform respawnPoint;

    [Header("UI 元素")]
    public TextMeshProUGUI countdownText;
    public Image blackScreen;

    [Header("摄像机控制")]
    public GameObject freeLookCamera;
    public GameObject fixedViewCamera;
    public bool switchBackToFreeLook = false;
    public float switchBackDelay = 3f;

    [Header("结局演出")]
    public AudioSource introSource;       // 播 Intro 的 AudioSource（5 秒）
    public AudioSource finaleSource;      // 播 Finale 的 AudioSource（2 秒）
    public ParticleSystem finaleVFX;
    public TextMeshProUGUI endingText;    // 显示 “You Did It!”

    private Coroutine countdownRoutine;
    private bool isCounting = false;

    void OnTriggerEnter(Collider other)
    {
        // 启动倒计时
        if (other.CompareTag("TriggerZone") && !isCounting)
        {
            countdownRoutine = StartCoroutine(StartCountdown());
        }

        // 取消倒计时并进入结局流程
        if ((other.CompareTag("CancelZone") || other.CompareTag("Ground_Level1")) && isCounting)
        {
            ForceCancelCountdown();

            if (freeLookCamera != null) freeLookCamera.SetActive(false);
            if (fixedViewCamera != null) fixedViewCamera.SetActive(true);

            StartCoroutine(PlayFinaleSequence());
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

    IEnumerator PlayFinaleSequence()
    {
        // Step 1: 播放 Intro 音效
        if (introSource != null)
        {
            introSource.Play();
            yield return new WaitForSeconds(introSource.clip.length);
        }

        // Step 2: 同时播放 Finale 音效 + 粒子
        if (finaleSource != null)
            finaleSource.Play();

        if (finaleVFX != null)
            finaleVFX.Play();

        if (finaleSource != null)
            yield return new WaitForSeconds(finaleSource.clip.length);

        // Step 3: 黑幕淡入
        yield return StartCoroutine(FadeToBlack(2f));

        // Step 4: 显示结局文字
        if (endingText != null)
        {
            endingText.text = "You Did It!";
            endingText.alpha = 1f;
        }

        Debug.Log("🎉 结局完成");
    }
}