using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VREnemyAI_HG : MonoBehaviour
{
    [Header("\uD83E\uDDCD\u200D\u2642\uFE0F 타겟 및 위치 설정")]
    public Transform player;                 // 플레이어 참조
    public Transform homePosition;           // 적의 복귀 지점

    [Header("\uD83D\uDC41\uFE0F 감지 및 전투 거리")]
    public float detectRange = 15f;          // 감지 거리
    public float attackRange = 10f;          // 공격 거리
    public float returnRange = 20f;          // 추격 포기 거리

    [Header("\uD83D\uDD2B 전투 설정")]
    public int maxAmmo = 7;                  // 탄창 수
    public float reloadTime = 2.5f;          // 재장전 시간

    [Header("\uD83C\uDF0F 순찰 설정")]
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    private Animator animator;
    private SGShotCtrl shotCtrl;

    private int currentPatrolIndex = 0;
    private int ammo;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        shotCtrl = GetComponentInChildren<SGShotCtrl>();
        ammo = maxAmmo;

        SetAnimState(true, false, false, false, false, false); // 시작은 순찰
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool inDetectRange = distance <= detectRange;
        bool inAttackRange = distance <= attackRange;
        bool outOfChaseRange = distance > returnRange;

        if (inDetectRange && !inAttackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            SetAnimState(false, true, false, false, false, false); // 추격
        }
        else if (inAttackRange && agent.remainingDistance < 1.0f)
        {
            agent.isStopped = true;
            agent.ResetPath(); // 이동 강제 중단
            transform.LookAt(player);
            SetAnimState(false, false, true, false, false, false); // 공격

            if (!shotCtrl._shooting)
            {
                shotCtrl.Shooting = true;
                ammo--;
                Debug.Log("\uD83D\uDD25 공격! 남은 탄약: " + ammo);

                if (ammo <= 0)
                {
                    StartCoroutine(ReloadRoutine());
                }
            }
        }
        else if (outOfChaseRange)
        {
            SetAnimState(false, false, false, false, true, false); // 추격 포기 → 복귀
            agent.SetDestination(homePosition.position);

            if (Vector3.Distance(transform.position, homePosition.position) < 0.5f)
            {
                SetAnimState(true, false, false, false, false, false); // 순찰 복귀
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        agent.isStopped = false;
        Transform targetPoint = patrolPoints[currentPatrolIndex];

        // 다음 지점으로 전환 조건
        if (agent.remainingDistance <= 0.3f && !agent.pathPending)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        // 확실한 이동 보장
        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        SetAnimState(true, false, false, false, false, false); // 순찰
    }

    IEnumerator ReloadRoutine()
    {
        SetAnimState(false, false, false, true, false, false); // 재장전
        yield return new WaitForSeconds(reloadTime);
        ammo = maxAmmo;
        SetAnimState(false, false, true, false, false, false); // 다시 공격으로
    }

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        SetAnimState(false, false, false, false, false, true); // 사망 상태
    }

    void SetAnimState(bool patrol, bool chase, bool attack, bool reload, bool ret, bool dead)
    {
        animator.SetBool("isPatrolling", patrol);
        animator.SetBool("isChasing", chase);
        animator.SetBool("isAttacking", attack);
        animator.SetBool("isReloading", reload);
        animator.SetBool("isReturning", ret);
        animator.SetBool("isDead", dead);
    }
}


