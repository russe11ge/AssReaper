using UnityEngine;
using UnityEngine.AI;

public class ChaseTrigger : MonoBehaviour
{
    public NavMeshAgent npcAgent;   // NPC的NavMeshAgent组件
    public Transform followPoint;   // 新增的追踪目标点 (地面附近)
    private bool playerInZone = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            npcAgent.ResetPath();
        }
    }

    void Update()
    {
        if (playerInZone)
        {
            npcAgent.SetDestination(followPoint.position);
        }
    }
}