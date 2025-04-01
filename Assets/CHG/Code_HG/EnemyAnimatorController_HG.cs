using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorController : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float chaseRange = 10.0f; // 플레이어를 추적할 범위
    public float stoppingDistance = 2.0f; // 플레이어와 멈추는 거리
    public Animator animator; // 적의 Animator
    private NavMeshAgent agent; // NavMeshAgent를 통한 이동 제어

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트 가져오기
    }

    void Update()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어 발견 시 (추적 범위 안에 있음)
        if (distanceToPlayer < chaseRange)
        {
            // 플레이어를 따라감
            agent.SetDestination(player.position);

            // 걷기 애니메이션 활성화
            if (distanceToPlayer > stoppingDistance)
            {
                animator.SetBool("IsWalking", true); // 걷기 애니메이션 시작
                animator.SetBool("SeePlayer", true); // 플레이어를 발견
            }
            else
            {
                // 플레이어와 가까우면 멈춤
                animator.SetBool("IsWalking", false); // 걷기 멈춤
            }
        }
        else
        {
            // 플레이어를 놓쳤을 때
            animator.SetBool("SeePlayer", false);
            animator.SetBool("IsWalking", false);
            agent.ResetPath(); // 이동 중지
        }

        // 플레이어를 향해 회전
        if (distanceToPlayer <= chaseRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0f, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
        }
    }
}

