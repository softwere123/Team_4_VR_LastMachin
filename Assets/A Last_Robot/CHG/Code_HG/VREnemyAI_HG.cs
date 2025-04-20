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
    public float attackRange = 10f;        // 🔫 공격 거리
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

        // 발사 콜백 등록
        if (shotCtrl != null)
        {
            shotCtrl.onProjectileFired += HandleProjectileFired;
        }
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool isPlayerVisible = distance <= detectRange;
        bool inAttackRange = distance <= attackRange;
        bool isTooFar = distance > maxChaseDistance;

        if (isReloading)
        {
            animator.SetBool("isReloading", true);
            return;
        }

        if (isTooFar)
        {
            agent.SetDestination(homePosition.position);
            animator.SetBool("isReturning", true);
            animator.SetBool("isChasing", false);
            animator.SetBool("isAttacking", false);
            shotCtrl.Shooting = false;
            return;
        }

        animator.SetBool("isReturning", false);

        if (isPlayerVisible)
        {
            agent.SetDestination(player.position);
            animator.SetBool("isChasing", true);

            if (inAttackRange)
            {
                animator.SetBool("isAttacking", true);
                agent.isStopped = true;

                // ✅ 한 번만 발사 트리거 (중복 방지)
                if (!shotCtrl._shooting)
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
    /// ✅ 탄환 발사 시 호출됨 (SGLinearShot_HG에서 콜백 실행)
    /// </summary>
    private void HandleProjectileFired()
    {
        ammo--;
        Debug.Log("💣 탄 발사됨! 남은 탄약: " + ammo);

        if (ammo <= 0)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    /// <summary>
    /// ⏳ 장전 루틴
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
        Debug.Log("✅ 장전 완료! 탄약: " + ammo);
    }

    /// <summary>
    /// 💢 데미지 처리
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("🔥 피해 입음! 현재 체력: " + currentHealth);

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
        Debug.Log("💀 적 사망");

        StartCoroutine(DieCleanup());
    }

    IEnumerator DieCleanup()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}



