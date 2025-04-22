using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class VREnemyAI_HG : MonoBehaviour
{
    [Header("타겟 및 거리")]
    public Transform player; // 🎯 플레이어 Transform - 적이 추적할 대상
    public Transform[] patrolPoints; // 🛣️ 순찰 경로 지점들
    public float detectRange = 15f; // 👁️ 플레이어를 감지하는 범위
    public float attackRange = 8f; // 🔫 공격이 가능한 거리

    [HideInInspector]
    public float returnRange = 20f; // 🔕 리턴 기능은 현재 사용하지 않음 (숨김 처리)

    [Header("시야각")]
    [Range(0, 360)] public float viewAngle = 120f; // 👀 적의 시야각도

    [Header("전투 설정")]
    public float attackCooldown = 1f; // ⏱️ 공격 간의 쿨다운 시간
    public int maxAmmo = 7; // 💣 한 사이클 당 최대 탄 수
    public float reloadTime = 2f; // 🔄 재장전 시간

    [Header("감지 구역")]
    public DetectionZone_HG detectionZone; // 📦 플레이어가 들어왔는지 감지하는 트리거

    [Header("탄막 시스템")]
    public SGShotCtrl shotCtrl; // 🔥 총알 발사를 담당하는 SGShotCtrl

    private NavMeshAgent agent; // 🧭 AI 경로 탐색 제어
    private Animator animator; // 🎞️ 애니메이션 제어

    private int currentPatrolIndex = 0; // 순찰 경로 인덱스
    private bool isDead = false; // ☠️ 사망 여부
    private int currentAmmo; // 📦 현재 탄 수
    private float lastAttackTime; // 🕓 마지막으로 공격한 시간 기록
    private bool isReloading = false; // 🔄 재장전 중 여부

    void Start()
    {
        // 필요한 컴포넌트 가져오기
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentAmmo = maxAmmo;
        lastAttackTime = -999f;

        agent.stoppingDistance = 0.01f; // 거의 붙을 때 정지

        // SGShotCtrl 이벤트 연결
        if (shotCtrl != null)
        {
            shotCtrl.onProjectileFired += OnProjectileFired;
            Debug.Log("✅ SGShotCtrl 이벤트 등록 완료");
        }

        // 시작 상태는 순찰로
        SetAnimState(true, false, false, false, /* false */ false, false);

        // 순찰 경로 시작 지점으로 이동
        if (patrolPoints.Length >= 2)
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Update()
    {
        if (isDead || player == null) return;

        // 플레이어 거리와 시야 확인
        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
        bool inViewAngle = angleToPlayer <= viewAngle * 0.5f;
        bool inZone = detectionZone != null && detectionZone.playerInside;

        // 모든 감지 조건 만족 시 플레이어 발견
        bool playerDetected = (distance <= detectRange) && inViewAngle && inZone;

        if (isReloading) return;

        // 공격 거리일 때
        if (playerDetected && distance <= attackRange)
        {
            agent.isStopped = true;
            agent.ResetPath();

            // 플레이어 방향으로 회전
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            targetRotation *= Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7.5f * Time.deltaTime);

            SetAnimState(false, false, true, false, /* false */ false, false);
            HandleAttack();
        }
        // 감지되었지만 아직 공격 거리 아님
        else if (playerDetected && distance > attackRange /* && distance <= returnRange */)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            SetAnimState(false, true, false, false, /* false */ false, false);
        }
        // 감지되지 않음 → 순찰
        else if (!playerDetected /* || distance > returnRange */)
        {
            SetAnimState(true, false, false, false, /* false */ false, false);
            Patrol();

            // 🔕 리턴 기능 비활성화
            /*
            if (!agent.hasPath)
            {
                SetAnimState(false, false, false, false, true, false);
                agent.SetDestination(returnPosition);
            }
            */
        }
    }

    void HandleAttack()
    {
        // 쿨다운 + 탄약 체크
        if (Time.time - lastAttackTime >= attackCooldown && currentAmmo > 0)
        {
            lastAttackTime = Time.time;
            Debug.Log("🧠 공격 상태 유지 (탄 발사는 애니메이션에서)");
        }
    }

    void OnProjectileFired()
    {
        currentAmmo--;
        Debug.Log($"📦 탄약 차감됨 → 남은 탄: {currentAmmo}");
    }

    public void FireBullet()
    {
        // 애니메이션 이벤트에서 호출됨
        if (currentAmmo > 0 && !isReloading)
        {
            Debug.Log("🔫 애니메이션 중에 발사!");
            shotCtrl?.StartShot(); // 실제 발사
        }
    }

    public void OnAttackAnimationEnd()
    {
        // 탄약 없음 → 재장전
        if (currentAmmo <= 0 && !isReloading)
        {
            Debug.Log("🎯 애니메이션 종료 후 → 재장전 시작");
            StartReload();
        }
    }

    void StartReload()
    {
        isReloading = true;
        SetAnimState(false, false, false, true, /* false */ false, false);
        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        Debug.Log("🔁 장전 중...");
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        Debug.Log($"✅ 장전 완료 → 탄약 {currentAmmo} 발 복구");
        SetAnimState(false, false, true, false, /* false */ false, false);
    }

    void Patrol()
    {
        if (patrolPoints.Length < 2) return;

        // 순찰 포인트에 도착
        if (!agent.pathPending && agent.remainingDistance <= 0.5f && agent.velocity.sqrMagnitude < 0.05f)
        {
            agent.ResetPath();
            currentPatrolIndex = currentPatrolIndex == 0 ? 1 : 0; // 왕복 방식
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        // 순찰 중 회전 처리
        Vector3 toTarget = patrolPoints[currentPatrolIndex].position - transform.position;
        if (toTarget.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(toTarget.x, 0, toTarget.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }

    void SetAnimState(bool patrol, bool chase, bool attack, bool reload, /* bool returning */ bool unused, bool dead)
    {
        animator.SetBool("isPatrolling", patrol);
        animator.SetBool("isChasing", chase);
        animator.SetBool("isAttacking", attack);
        animator.SetBool("isReloading", reload);
        // animator.SetBool("isReturning", returning); // 🔕 리턴 상태 비활성화
        animator.SetBool("isDead", dead);
    }

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        SetAnimState(false, false, false, false, /* false */ false, true);
    }
}






