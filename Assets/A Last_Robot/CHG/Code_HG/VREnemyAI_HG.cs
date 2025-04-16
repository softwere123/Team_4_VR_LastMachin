using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI_HG : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform player;
    private Animator animator;
    private Vector3 initialPosition;

    [Header("탐지 및 공격 거리")]
    public float detectionRange = 15f;
    public float attackRange = 7f;
    public float minAttackDistance = 5f;

    [Header("탄약 및 리로드")]
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    [Header("체력")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    private bool isAttacking = false;

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
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        bool seePlayer = distanceToPlayer <= detectionRange;
        animator.SetBool("SeePlayer", seePlayer);

        // 플레이어를 못 보고 있고, 원래 자리에서 멀어졌고, 공격 중이 아니면 복귀
        if (!seePlayer && Vector3.Distance(transform.position, initialPosition) > 0.5f && !isAttacking)
        {
            agent.isStopped = false;
            agent.SetDestination(initialPosition);
            animator.SetBool("IsWalking", true);
            animator.SetBool("CanShoot", false);
        }

        // 플레이어 감지 중
        if (seePlayer)
        {
            if (distanceToPlayer <= attackRange && distanceToPlayer >= minAttackDistance)
            {
                agent.isStopped = true;
                agent.SetDestination(transform.position);
                agent.velocity = Vector3.zero;

                animator.SetBool("IsWalking", false);

                if (!isReloading && currentAmmo > 0 && !isAttacking)
                {
                    animator.SetBool("CanShoot", true);
                    animator.SetBool("Reload", false);
                    Shoot();
                }
                else if (!isReloading && currentAmmo <= 0)
                {
                    StartCoroutine(Reload());
                }
            }
            else if (!isAttacking)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetBool("IsWalking", true);
                animator.SetBool("CanShoot", false);
            }
        }

        // 복귀 완료 시 Idle
        if (!seePlayer && Vector3.Distance(transform.position, initialPosition) <= 0.5f)
        {
            animator.SetBool("IsWalking", false);
            isAttacking = false;
        }
    }

    void Shoot()
    {
        if (currentAmmo <= 0) return;

        Debug.Log("[Enemy] 공격! 남은 탄약: " + currentAmmo);
        currentAmmo--;
        isAttacking = true;
        animator.SetTrigger("Shoot");

        if (currentAmmo <= 0)
        {
            // 탄약이 없으면 Reload로 전환
            animator.SetBool("Reload", true); // 반드시 Animator에 Reload == true 조건이 있어야 함
            animator.SetBool("CanShoot", false);
        }

        // 애니메이션 종료까지 기다렸다가 다시 공격 가능하게
        StartCoroutine(ResetAttackAfterDelay());
    }

    IEnumerator ResetAttackAfterDelay()
    {
        // Attack 애니메이션 길이에 맞게 조절
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
        agent.isStopped = false;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("Reload", true);
        animator.SetBool("CanShoot", false);

        Debug.Log("[Enemy] 장전 중...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        animator.SetBool("Reload", false);
        animator.SetBool("CanShoot", true);

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
        animator.SetBool("Die", true);
        Debug.Log("[Enemy] 사망!");
    }
}
