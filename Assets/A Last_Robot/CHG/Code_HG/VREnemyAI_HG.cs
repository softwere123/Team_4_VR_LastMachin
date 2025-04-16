using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI_HG : MonoBehaviour
{
    private NavMeshAgent agent;                  // 적의 경로 탐색 및 이동 제어용 NavMeshAgent
    public Transform player;                     // 플레이어 위치 참조
    private Animator animator;                   // 애니메이션 제어용 Animator
    private Vector3 initialPosition;             // 적이 시작한 위치 (복귀용)

    [Header("탐지 및 공격 거리")]
    public float detectionRange = 15f;           // 플레이어를 감지할 수 있는 거리
    public float attackRange = 7f;               // 공격이 가능한 최대 거리
    public float minAttackDistance = 5f;         // 너무 가까우면 공격하지 않도록 설정하는 최소 거리

    [Header("탄약 및 리로드")]
    public int maxAmmo = 10;                     // 한 탄창당 최대 탄약 수
    private int currentAmmo;                     // 현재 남은 탄약 수
    public float reloadTime = 2f;                // 리로드 시간(초)
    private bool isReloading = false;            // 현재 리로드 중인지 여부

    [Header("체력")]
    public int maxHealth = 100;                  // 적의 최대 체력
    private int currentHealth;                   // 현재 체력
    private bool isDead = false;                 // 사망 여부

    private bool isAttacking = false;            // 현재 공격 중인지 여부

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;    // 시작 위치 저장
        currentAmmo = maxAmmo;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;                      // 사망 상태면 동작 중지

        float distanceToPlayer = Vector3.Distance(transform.position, player.position); // 플레이어까지의 거리 계산
        bool seePlayer = distanceToPlayer <= detectionRange;                            // 플레이어 감지 여부

        animator.SetBool("SeePlayer", seePlayer); // 애니메이터에 감지 상태 전달

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 현재 애니메이션 상태 확인

        // 현재 공격 중인데 너무 가까워졌다면 → 강제로 추적 상태로 전환
        if (stateInfo.IsName("Attack") && distanceToPlayer < minAttackDistance)
        {
            animator.Play("Chase", 0, 0f);              // 애니메이션 상태를 즉시 "Chase"로 전환
            animator.SetBool("CanShoot", false);        // 공격 중지
            animator.SetBool("Reload", false);          // 리로드 중지
            isAttacking = false;

            agent.isStopped = false;
            agent.SetDestination(player.position);      // 플레이어 위치로 이동
            animator.SetBool("IsWalking", true);        // 추적 애니메이션 재생
            return;                                     // 더 이상 아래 코드 실행하지 않음
        }

        // 너무 가까우면 공격/리로드 하지 않고 걷기 상태로 유지
        if (seePlayer && distanceToPlayer < minAttackDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            animator.SetBool("IsWalking", true);
            animator.SetBool("CanShoot", false);
            animator.SetBool("Reload", false);

            isAttacking = false;
            isReloading = false;
            return;
        }

        // 체력이 0 이하가 되면 사망 처리
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // 공격 가능 거리라면
        if (seePlayer && distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            agent.SetDestination(transform.position);   // 그 자리에 멈추게 함
            agent.velocity = Vector3.zero;              // 움직임 초기화

            animator.SetBool("IsWalking", false);       // 걷기 상태 중지

            // 공격 가능 상태
            if (!isReloading && currentAmmo > 0 && !isAttacking)
            {
                animator.SetBool("CanShoot", true);
                animator.SetBool("Reload", false);
                Shoot(); // 총 발사
            }
            // 탄약이 없고 리로드 중이 아니라면 → 리로드 시작
            else if (!isReloading && currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
        }
        // 감지 중이지만 공격 거리 밖이라면 → 플레이어 추적
        else if (seePlayer)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("IsWalking", true);
            animator.SetBool("CanShoot", false);
        }

        // 감지하지 못했고, 원래 위치에 거의 도착했을 때 → Idle 상태
        if (!seePlayer && Vector3.Distance(transform.position, initialPosition) <= 0.5f)
        {
            animator.SetBool("IsWalking", false);
            isAttacking = false;
        }

        // 감지하지 못했고, 아직 복귀 중이라면 → 복귀
        if (!seePlayer && Vector3.Distance(transform.position, initialPosition) > 0.5f && !isAttacking)
        {
            agent.isStopped = false;
            agent.SetDestination(initialPosition);
            animator.SetBool("IsWalking", true);
            animator.SetBool("CanShoot", false);
        }
    }

    // 총 발사 처리
    void Shoot()
    {
        if (currentAmmo <= 0) return;

        // 발사 직전에 플레이어 방향으로 즉시 회전
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f; // 수직 회전 제거
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = lookRot;
        }

        Debug.Log("[Enemy] 공격! 남은 탄약: " + currentAmmo);
        currentAmmo--;
        isAttacking = true;

        animator.SetTrigger("Shoot"); // 공격 애니메이션 실행

        if (currentAmmo <= 0)
        {
            animator.SetBool("Reload", true);   // 탄약 다 떨어졌으면 Reload 상태로 전환
            animator.SetBool("CanShoot", false);
        }

        StartCoroutine(ResetAttackAfterDelay()); // 공격 상태 해제 대기 코루틴
    }

    // 공격 애니메이션이 끝난 후 상태 초기화
    IEnumerator ResetAttackAfterDelay()
    {
        yield return new WaitForSeconds(1.0f); // 공격 애니메이션 길이만큼 대기
        isAttacking = false;
        agent.isStopped = false;
    }

    // 장전 처리 코루틴
    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("Reload", true);     // Reload 상태 애니메이션 진입
        animator.SetBool("CanShoot", false);

        Debug.Log("[Enemy] 장전 중...");

        yield return new WaitForSeconds(reloadTime); // 장전 시간 대기

        currentAmmo = maxAmmo;              // 탄약 완충
        isReloading = false;

        animator.SetBool("Reload", false);  // 리로드 종료
        animator.SetBool("CanShoot", true); // 다시 공격 가능
        Debug.Log("[Enemy] 장전 완료. 탄약: " + currentAmmo);
    }

    // 피해 처리
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

    // 사망 처리
    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetBool("Die", true); // 사망 애니메이션 실행
        Debug.Log("[Enemy] 사망!");
    }
}


