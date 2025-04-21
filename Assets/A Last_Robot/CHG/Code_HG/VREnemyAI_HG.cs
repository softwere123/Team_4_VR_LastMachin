using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class VREnemyAI_HG : MonoBehaviour
{
    [Header("타겟 및 위치")]
    public Transform player;               // 🎯 플레이어 위치
    public Transform homePosition;         // 🏠 복귀 지점

    [Header("감지 및 전투 거리")]
    public float detectRange = 15f;        // 👀 감지 거리
    public float attackRange = 8f;         // 🔫 공격 거리
    public float maxChaseDistance = 25f;   // 📏 너무 멀어졌을 때 복귀

    [Header("전투 설정")]
    public int maxAmmo = 7;                // 💥 탄약 수
    public float reloadTime = 2.5f;        // ⏱ 장전 시간
    public int maxHealth = 100;            // ❤️ 최대 체력

    private int currentHealth;
    private int ammo;
    private bool isReloading = false;
    private bool isDead = false;

    private NavMeshAgent agent;
    private Animator animator;
    private SGShotCtrl shotCtrl;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        shotCtrl = GetComponent<SGShotCtrl>();

        currentHealth = maxHealth;
        ammo = maxAmmo;

        // 탄 발사 이벤트 등록
        shotCtrl.onProjectileFired += HandleProjectileFired;
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool isPlayerVisible = distance <= detectRange;
        bool inAttackRange = distance <= attackRange;
        bool isTooFar = distance > maxChaseDistance;

        // 🛑 장전 중이면 공격 외의 행동 중지
        if (isReloading)
        {
            animator.SetBool("isReloading", true);
            return;
        }

        // ⏪ 너무 멀어지면 복귀
        if (isTooFar)
        {
            animator.SetBool("isReturning", true);
            animator.SetBool("isChasing", false);
            animator.SetBool("isAttacking", false);
            shotCtrl.Shooting = false;

            // ✅ 복귀 위치로 이동
            if (!agent.pathPending && agent.destination != homePosition.position)
            {
                agent.isStopped = false;
                agent.SetDestination(homePosition.position);
                Debug.Log("🏠 복귀 중 → 목적지: " + homePosition.position);
            }

            return;
        }

        animator.SetBool("isReturning", false);

        // 👀 플레이어가 감지되었을 때
        if (isPlayerVisible)
        {
            // ✅ 목적지가 바뀌었을 때만 재설정
            if (!agent.pathPending && agent.destination != player.position)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                Debug.Log("🎯 추적 중 → 목적지: " + player.position);
            }

            animator.SetBool("isChasing", true);

            if (inAttackRange)
            {
                animator.SetBool("isAttacking", true);
                agent.isStopped = true; // 공격 시 정지

                if (!shotCtrl._shooting && ammo > 0)
                {
                    shotCtrl.Shooting = true;
                }
            }
            else
            {
                animator.SetBool("isAttacking", false);
                shotCtrl.Shooting = false;
                agent.isStopped = false;
            }
        }
        else
        {
            animator.SetBool("isChasing", false);
            animator.SetBool("isAttacking", false);
            shotCtrl.Shooting = false;
        }
    }

    /// <summary>
    /// 💣 탄 발사 이벤트 핸들링
    /// </summary>
    void HandleProjectileFired()
    {
        if (isReloading || isDead) return;

        ammo--;
        Debug.Log("🔫 발사됨! 남은 탄약: " + ammo);

        if (ammo <= 0)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    /// <summary>
    /// 🔁 장전 처리 루틴
    /// </summary>
    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        shotCtrl.Shooting = false;
        animator.SetBool("isReloading", true);

        Debug.Log("🔄 장전 중...");

        yield return new WaitForSeconds(reloadTime);

        ammo = maxAmmo;
        isReloading = false;
        animator.SetBool("isReloading", false);

        Debug.Log("✅ 장전 완료!");
    }

    /// <summary>
    /// 💥 피해 처리
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("😵 피해 입음: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// ☠️ 사망 처리
    /// </summary>
    private void Die()
    {
        isDead = true;

        animator.SetBool("isDead", true);
        shotCtrl.Shooting = false;
        agent.isStopped = true;

        Debug.Log("☠️ 사망!");

        StartCoroutine(DieCleanup());
    }

    /// <summary>
    /// 🧼 사망 후 비활성화
    /// </summary>
    IEnumerator DieCleanup()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}







