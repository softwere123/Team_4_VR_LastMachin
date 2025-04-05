using UnityEngine;
using UnityEngine.AI; // NavMeshAgent를 사용하기 위해 필요

public class EnemyAI : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float speed = 3.5f; // 적의 이동 속도
    public float stoppingDistance = 2.0f; // 플레이어와의 최소 거리

    private NavMeshAgent agent; // 적의 NavMesh 에이전트

    void Start()
    {
        // NavMeshAgent 컴포넌트 가져오기
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed; // NavMeshAgent의 이동 속도 설정
    }

    void Update()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어가 stoppingDistance보다 멀리 있으면 추적
        if (distanceToPlayer > stoppingDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath(); // 멈추기
        }

        // 플레이어를 항상 바라보기
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
