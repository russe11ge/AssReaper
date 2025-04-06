using UnityEngine;

public class FanTrigger : MonoBehaviour
{
    private Animator fanAnimator;

    void Start()
    {
        // 获取父物体上的 Animator
        fanAnimator = GetComponentInParent<Animator>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("风扇被射中了！");

            if (fanAnimator != null)
            {
                fanAnimator.SetBool("isSpinning", true);
            }

            // 玩家站上来，设置为风扇的子物体
            collision.transform.SetParent(fanAnimator.transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 玩家离开风扇，取消父子关系
            collision.transform.SetParent(null);
        }
    }
}
