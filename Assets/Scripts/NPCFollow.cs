using UnityEngine;
using UnityEngine.AI;

public class NPCFollow : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        NavMeshHit hit;
        // 自动修正玩家位置到导航网格上方
        if (NavMesh.SamplePosition(player.position, out hit, 10.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}