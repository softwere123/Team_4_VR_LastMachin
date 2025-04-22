using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class VREnemyAI_HG : MonoBehaviour
{
    [Header("타겟 및 거리")]
    public Transform player; // 🎯 추적할 플레이어의 Transform
    public Transform[] patrolPoints; // 🛣️ 순찰 지점 배열
    public float detectRange = 15f; // 👁️ 플레이어를 감지할 거리
    public float attackRange = 8f; // 🔫 공격을 시작할 거리
    public float returnRange = 20f; // 🔁 추격을 포기하고 복귀할 거리

    [Header("시야각")]
    [Range(0, 360)] public float viewAngle = 120f; // 🔍 플레이어를 감지할 수 있는 시야각

    [Header("전투 설정")]
    public float attackCooldown = 1f; // ⏱️ 공격 간 쿨타임
    public int maxAmmo = 7; // 💣 최대 탄 수
    public float reloadTime = 2f; // 🔄 재장전 시간

    [Header("감지 구역")]
    public DetectionZone_HG detectionZone; // 📦 감지를 위한 트리거 박스

    [Header("탄막 시스템")]
    public SGShotCtrl shotCtrl; // 🔥 SGShotCtrl 컴포넌트 참조

    private NavMeshAgent agent; // 🧭 AI 이동 제어
    private Animator animator; // 🎬 애니메이션 제어

    private int currentPatrolIndex = 0; // 📍 순찰 지점 인덱스
    private bool isDead = false; // ☠️ 사망 여부
    private int currentAmmo; // 📦 현재 남은 탄
    private float lastAttackTime; // 🕓 마지막 공격 시각

    private bool isReloading = false; // 🔁 현재 재장전 중인지 여부

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // 네비메시 에이전트 초기화
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 초기화
        currentAmmo = maxAmmo; // 시작 시 탄약 풀
        lastAttackTime = -999f; // 첫 공격 즉시 가능하게 설정

        agent.stoppingDistance = 0.01f; // 도착 후 거의 붙게 정지

        if (shotCtrl != null)
        {
            shotCtrl.onProjectileFired += OnProjectileFired; // SGShotCtrl에서 탄 발사 이벤트 구독
            Debug.Log("✅ SGShotCtrl 이벤트 등록 완료");
        }

        SetAnimState(true, false, false, false, /* false */ false, false); // 초기 상태를 순찰로 설정

        if (patrolPoints.Length >= 2)
            agent.SetDestination(patrolPoints[currentPatrolIndex].position); // 순찰 시작
    }

    void Update()
    {
        if (isDead || player == null) return; // 사망하거나 플레이어 없으면 동작 중지

        float distance = Vector3.Distance(transform.position, player.position); // 플레이어와의 거리 계산
        Vector3 dirToPlayer = (player.position - transform.position).normalized; // 플레이어 방향
        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer); // 시야각 내인지 확인
        bool inViewAngle = angleToPlayer <= viewAngle * 0.5f; // 시야각 범위 체크
        bool inZone = detectionZone != null && detectionZone.playerInside; // DetectionZone 안에 있는지

        bool playerDetected = (distance <= detectRange) && inViewAngle && inZone; // 세 조건 모두 만족 시 감지

        if (isReloading) return; // 재장전 중이면 아무것도 하지 않음

        if (playerDetected && distance <= attackRange)
        {
            agent.isStopped = true; // 이동 중지
            agent.ResetPath(); // 현재 경로 초기화

            Vector3 direction = (player.position - transform.position).normalized; // 방향 재계산
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // 회전 대상
            targetRotation *= Quaternion.Euler(0, 0, 0); // 약간 보정
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7.5f * Time.deltaTime); // 부드럽게 회전

            SetAnimState(false, false, true, false, /* false */ false, false); // 공격 상태
            HandleAttack(); // 공격 로직 실행
        }
        else if (playerDetected && distance > attackRange && distance <= returnRange)
        {
            agent.isStopped = false; // 이동 재시작
            agent.SetDestination(player.position); // 플레이어 추적
            SetAnimState(false, true, false, false, /* false */ false, false); // 추격 상태
        }
        else if (!playerDetected || distance > returnRange)
        {
            SetAnimState(true, false, false, false, /* false */ false, false); // 순찰 상태로
            Patrol(); // 순찰 루틴 실행

            // ⛔ 리턴 기능 (추후 활성화 가능)
            /*
            if (!agent.hasPath)
            {
                SetAnimState(false, false, false, false, true, false); // 리턴 상태
                agent.SetDestination(returnPosition); // 복귀 위치 이동
            }
            */
        }
    }

    void HandleAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown && currentAmmo > 0)
        {
            lastAttackTime = Time.time; // 공격 시간 갱신
            Debug.Log("🧠 공격 상태 유지 (탄 발사는 애니메이션에서)");
        }
    }

    void OnProjectileFired()
    {
        currentAmmo--; // 탄약 차감
        Debug.Log($"📦 탄약 차감됨 → 남은 탄: {currentAmmo}");
    }

    public void FireBullet()
    {
        if (currentAmmo > 0 && !isReloading)
        {
            Debug.Log("🔫 애니메이션 중에 발사!");
            shotCtrl?.StartShot(); // 실제 발사 명령
        }
    }

    public void OnAttackAnimationEnd()
    {
        if (currentAmmo <= 0 && !isReloading)
        {
            Debug.Log("🎯 애니메이션 종료 후 → 재장전 시작");
            StartReload(); // 재장전 루틴 시작
        }
    }

    void StartReload()
    {
        isReloading = true; // 재장전 상태 진입
        SetAnimState(false, false, false, true, /* false */ false, false); // 재장전 애니메이션
        StartCoroutine(ReloadRoutine()); // 재장전 처리 루틴 시작
    }

    IEnumerator ReloadRoutine()
    {
        Debug.Log("🔁 장전 중...");
        yield return new WaitForSeconds(reloadTime); // 재장전 시간 대기

        currentAmmo = maxAmmo; // 탄약 충전
        isReloading = false; // 재장전 종료

        Debug.Log($"✅ 장전 완료 → 탄약 {currentAmmo} 발 복구");
        SetAnimState(false, false, true, false, /* false */ false, false); // 다시 공격 상태
    }

    void Patrol()
    {
        if (patrolPoints.Length < 2) return; // 순찰 포인트 2개 미만이면 무시

        if (!agent.pathPending && agent.remainingDistance <= 0.5f && agent.velocity.sqrMagnitude < 0.05f)
        {
            agent.ResetPath(); // 경로 초기화
            currentPatrolIndex = currentPatrolIndex == 0 ? 1 : 0; // 왕복
            agent.SetDestination(patrolPoints[currentPatrolIndex].position); // 다음 지점 이동
        }

        Vector3 toTarget = patrolPoints[currentPatrolIndex].position - transform.position;
        if (toTarget.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(toTarget.x, 0, toTarget.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }

    void SetAnimState(bool patrol, bool chase, bool attack, bool reload, /* bool returning */ bool unused, bool dead)
    {
        animator.SetBool("isPatrolling", patrol); // 순찰 상태
        animator.SetBool("isChasing", chase); // 추격 상태
        animator.SetBool("isAttacking", attack); // 공격 상태
        animator.SetBool("isReloading", reload); // 재장전 상태
        // animator.SetBool("isReturning", returning); // ⛔ 리턴 상태 (비활성화)
        animator.SetBool("isDead", dead); // 사망 상태
    }

    public void Die()
    {
        isDead = true; // 사망 플래그
        agent.isStopped = true; // 이동 정지
        SetAnimState(false, false, false, false, /* false */ false, true); // 사망 애니메이션
    }
}




