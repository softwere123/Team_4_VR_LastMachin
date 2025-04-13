using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_HG : MonoBehaviour
{
    private NavMeshAgent agent;              // 네비메시 에이전트: 경로 탐색 및 이동 제어용
    public Transform player;                 // 플레이어 위치 참조
    private Animator animator;               // 애니메이터: 상태 전환 및 애니메이션 제어
    private Vector3 initialPosition;         // 처음 위치 저장: 플레이어를 놓쳤을 때 돌아갈 곳

    public float detectionRange = 15f;       // 플레이어를 감지할 거리
    public float attackRange = 7f;           // 플레이어를 공격할 수 있는 거리
    public float minAttackDistance = 5f;     // 너무 가까우면 공격하지 않음
    public float returnRange = 20f;          // 이 거리보다 멀어지면 복귀 시작

    public int maxAmmo = 5;                  // 최대 탄약 수
    private int currentAmmo;                 // 현재 탄약
    public float reloadTime = 2f;            // 재장전 시간 (초)
    private bool isReloading = false;        // 재장전 중 여부

    public int maxHealth = 100;              // 최대 체력
    private int currentHealth;               // 현재 체력
    private bool isDead = false;             // 사망 여부

    private bool isAttacking = false;        // 공격 중 여부 상태 저장

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        currentAmmo = maxAmmo;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return; // 사망 상태에서는 아무것도 하지 않음

        float distanceToPlayer = Vector3.Distance(transform.position, player.position); // 플레이어 거리 측정

        if (currentHealth <= 0)
        {
            Die(); // 체력 0 이하일 경우 사망 처리
            return;
        }

        bool seePlayer = distanceToPlayer <= detectionRange; // 감지 범위 내 플레이어 여부
        animator.SetBool("SeePlayer", seePlayer); // 애니메이터에 감지 여부 전달

        // 복귀 조건: 플레이어를 못 보고, 원래 자리에서 멀어졌고, 공격 중이 아닐 때만
        if (!seePlayer && Vector3.Distance(transform.position, initialPosition) > 0.5f && !isAttacking)
        {
            agent.isStopped = false;
            agent.SetDestination(initialPosition);
            animator.SetBool("IsWalking", true);
            animator.SetBool("CanShoot", false);
        }

        // 플레이어를 보고 있을 때
        if (seePlayer)
        {
            // 공격 가능 거리일 때
            if (distanceToPlayer <= attackRange && distanceToPlayer >= minAttackDistance)
            {
                // 공격 중: 위치 고정 및 이동 정지
                agent.isStopped = true;
                agent.SetDestination(transform.position); // 이동 목표를 자기 위치로 고정
                agent.velocity = Vector3.zero;             // 관성으로 인한 이동 제거

                animator.SetBool("IsWalking", false);

                isAttacking = true; // 공격 중 상태 플래그 설정

                if (currentAmmo > 0 && !isReloading)
                {
                    animator.SetBool("CanShoot", true);
                    animator.SetBool("Reload", false);
                    Shoot(); // 총 발사 로직 실행
                }
                else if (!isReloading)
                {
                    StartCoroutine(Reload()); // 탄약이 없으면 장전 시작
                }
            }
            // 공격 거리 밖이고, 공격 중이 아닐 때만 추격 허용
            else if (!isAttacking)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position); // 플레이어 추적 시작
                animator.SetBool("IsWalking", true);
                animator.SetBool("CanShoot", false);
            }
        }

        // 플레이어를 놓쳤고 원래 위치에 도착했을 때 Idle 전환
        if (!seePlayer && Vector3.Distance(transform.position, initialPosition) <= 0.5f)
        {
            animator.SetBool("IsWalking", false);
            isAttacking = false; // Idle로 돌아가면 공격 상태 초기화
        }
    }

    void Shoot()
    {
        Debug.Log("[Enemy] 공격! 남은 탄약: " + currentAmmo);
        currentAmmo--;
        animator.SetTrigger("Shoot"); // 공격 애니메이션 트리거 발동


        isAttacking = false;
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("Reload", true);
        animator.SetBool("CanShoot", false);

        Debug.Log("[Enemy] 장전 중...");

        yield return new WaitForSeconds(reloadTime); // 장전 시간 대기

        currentAmmo = maxAmmo;
        isReloading = false;

        Debug.Log("[Enemy] 장전 완료. 탄약: " + currentAmmo);
    }

    public void TakeDamage(int damage, string attackerName = "Unknown")
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"[Enemy] {attackerName}에게 피격됨! 현재 체력: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetBool("Die", true); // 사망 애니메이션 전환
        Debug.Log("[Enemy] 사망!");
    }


 // 어택 애니메이션 이벤트에서 호출할 함수
public void OnAttackEnd()
{
    // 공격 애니메이션이 끝난 후 다시 이동할 수 있도록 설정
    isAttacking = false;
    agent.isStopped = false;
}

}

