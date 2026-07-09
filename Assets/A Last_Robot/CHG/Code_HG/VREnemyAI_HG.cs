using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class VREnemyAI_HG : MonoBehaviour
{
    [Header("타겟 및 거리")]
    public Transform player; // 🎯 적이 추적할 플레이어
    public Transform[] patrolPoints; // 🛣️ 순찰 지점들 (왕복 방식)
    public float detectRange = 15f;
    public float attackRange = 8f;// 🔍 플레이어를 감지할 수 있는 거리
    [HideInInspector] public float returnRange = 20f; // 🔕 리턴 기능은 사용하지 않음

    [Header("시야각")]
    [Range(0, 360)] public float viewAngle = 120f; // 👁️ 플레이어 감지용 시야각

    [Header("전투 설정")]
    public float attackCooldown = 1f; // ⏱️ 공격 간 쿨타임
    public int maxAmmo = 7; // 💣 탄창 내 최대 탄 수
    public float reloadTime = 2f; // 🔄 재장전 시간

    [Header("감지 구역")]
    public DetectionZone_HG detectionZone; // 📦 감지 영역 트리거

    [Header("탄막 시스템")]
    public SGShotCtrl shotCtrl; // 🔫 총알 발사를 담당하는 시스템

    private NavMeshAgent agent; // 🧭 경로 탐색용 컴포넌트
    private Animator animator; // 🎞️ 애니메이션 제어

    private int currentPatrolIndex = 0; // 현재 순찰 중인 포인트 인덱스
    private bool isDead = false; // ☠️ 사망 여부
    private int currentAmmo; // 💥 현재 남은 탄 수
    private float lastAttackTime; // 🕓 마지막 공격 시간 기록
    private bool isReloading = false; // 🔃 재장전 중 여부

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 연결
        animator = GetComponent<Animator>(); // Animator 연결
        currentAmmo = maxAmmo; // 초기 탄 수 설정
        lastAttackTime = -999f; // 시작하자마자 공격 가능하도록 초기화
        agent.stoppingDistance = 0.01f; // 거의 도착했을 때 멈추도록 설정

        // SGShotCtrl 이벤트 연결 (탄이 실제 발사될 때 호출됨)
        if (shotCtrl != null)
        {
            shotCtrl.onProjectileFired += OnProjectileFired;
            Debug.Log("✅ SGShotCtrl 이벤트 연결 완료");
        }

        // 초기 상태를 순찰로 설정
        SetAnimState(true, false, false, false, false, false);

        // 순찰 지점이 2개 이상일 경우 시작 위치 지정
        if (patrolPoints.Length >= 2)
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Update()
    {
        if (isDead || player == null) return; // 사망했거나 플레이어 없음

        // 플레이어와의 거리, 방향, 시야 계산
        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
        bool inViewAngle = angleToPlayer <= viewAngle * 0.5f;
        bool inZone = detectionZone != null && detectionZone.playerInside;

        // 감지 조건: 거리 + 시야 + DetectionZone 안에 있어야 함
        bool playerDetected = (distance <= detectRange) && inViewAngle && inZone;

        if (isReloading) return; // 장전 중엔 아무것도 하지 않음

        // 공격 거리 내 && 감지 성공
        if (playerDetected && distance <= attackRange)
        {
            agent.isStopped = true; // 이동 멈춤
            agent.ResetPath(); // 경로 초기화

            // 플레이어 방향으로 회전 (몸통은 걷는 모습이 어색하지 않게 좌우로만 회전)
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7.5f * Time.deltaTime);

            // 총구는 몸통과 별개로 플레이어를 위아래까지 정확히 겨냥 (공중에 뜬 플레이어도 맞힐 수 있도록)
            AimShotCtrlAtPlayer();

            SetAnimState(false, false, true, false, false, false); // 공격 애니메이션 상태로
            HandleAttack(); // 실제 공격 로직 실행
        }
        // 감지되었지만 공격 거리 밖 → Chase 상태
        else if (playerDetected && distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            SetAnimState(false, true, false, false, false, false); // 추격 상태
        }
        // 감지되지 않음 (DetectionZone 밖 등) → Patrol 상태
        else
        {
            // 공격 상태였다면 즉시 종료하고 순찰 상태로
            if (animator.GetBool("isAttacking"))
            {
                Debug.Log("🚫 DetectionZone 벗어남 → 공격 중단 및 순찰 전환");
                SetAnimState(true, false, false, false, false, false);
            }

            Patrol(); // 순찰 로직 실행
        }
    }

    // 총구(shotCtrl)를 몸통 회전과 별개로 플레이어를 향해 위아래까지 겨냥한다.
    // SGProjectile이 발사 순간 이 회전을 그대로 물려받아 탄환 방향을 결정하므로,
    // 여기서 정확히 겨냥해둬야 플레이어가 공중에 있어도 맞힐 수 있다.
    void AimShotCtrlAtPlayer()
    {
        if (shotCtrl == null)
            return;

        Vector3 aimDirection = (player.position - shotCtrl.transform.position).normalized;
        if (aimDirection.sqrMagnitude < 0.0001f)
            return;

        Quaternion aimRotation = Quaternion.LookRotation(aimDirection);
        shotCtrl.transform.rotation = Quaternion.Slerp(shotCtrl.transform.rotation, aimRotation, 10f * Time.deltaTime);
    }

    // 공격 처리: 탄 수가 남았고 쿨타임이 지났다면 공격
    void HandleAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown && currentAmmo > 0)
        {
            lastAttackTime = Time.time;
            Debug.Log("🧠 공격 조건 충족 → 공격 준비 완료");
        }
    }

    // SGShotCtrl에서 실제 탄이 발사되었을 때 호출되는 콜백
    void OnProjectileFired()
    {
        currentAmmo--;
        Debug.Log($"📦 탄약 차감됨 → 남은 탄: {currentAmmo}");
    }

    // 애니메이션 이벤트에서 호출 → 발사 처리
    public void FireBullet()
    {
        if (currentAmmo > 0 && !isReloading)
        {
            Debug.Log("🔫 애니메이션 중 발사!");
            shotCtrl?.StartShot();
        }
    }

    // 공격 애니메이션 종료 시 호출 → 탄약 없으면 장전 시작
    public void OnAttackAnimationEnd()
    {
        if (currentAmmo <= 0 && !isReloading)
        {
            Debug.Log("🎯 탄약 없음 → 장전 시작");
            StartReload();
        }
    }

    // 장전 시작
    void StartReload()
    {
        isReloading = true;
        SetAnimState(false, false, false, true, false, false); // 장전 상태
        StartCoroutine(ReloadRoutine());
    }

    // 장전 처리 루틴
    IEnumerator ReloadRoutine()
    {
        Debug.Log("🔁 장전 중...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log($"✅ 장전 완료 → 탄약 {currentAmmo} 발 복구");

        // 장전 후 바로 공격 상태로 전환
        SetAnimState(false, false, true, false, false, false);
    }

    // 순찰 처리 로직
    void Patrol()
    {
        if (patrolPoints.Length < 2) return;

        if (!agent.pathPending && agent.remainingDistance <= 0.5f && agent.velocity.sqrMagnitude < 0.05f)
        {
            agent.ResetPath();
            currentPatrolIndex = currentPatrolIndex == 0 ? 1 : 0; // 왕복 순찰
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        // 순찰 중 회전 자연스럽게
        Vector3 toTarget = patrolPoints[currentPatrolIndex].position - transform.position;
        if (toTarget.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(toTarget.x, 0, toTarget.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }

    // 애니메이션 파라미터 설정 함수
    void SetAnimState(bool patrol, bool chase, bool attack, bool reload, bool unused, bool dead)
    {
        animator.SetBool("isPatrolling", patrol);
        animator.SetBool("isChasing", chase);
        animator.SetBool("isAttacking", attack);
        animator.SetBool("isReloading", reload);
        animator.SetBool("isDead", dead);
    }

    // 사망 처리 함수
    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        SetAnimState(false, false, false, false, false, true);
    }
}





